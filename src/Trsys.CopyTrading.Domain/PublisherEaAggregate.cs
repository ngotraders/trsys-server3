﻿using EventFlow.Aggregates;
using System.Collections.Generic;
using System.Linq;

namespace Trsys.CopyTrading.Domain
{
    public class PublisherEaAggregate : AggregateRoot<PublisherEaAggregate, PublisherEaId>,
        IEmit<PublisherEaRegisteredEvent>,
        IEmit<PublisherEaOrderTextUpdatedEvent>,
        IEmit<PublisherEaOpenedOrderEvent>,
        IEmit<PublisherEaClosedOrderEvent>
    {
        public SecretKey Key { get; private set; }
        public List<PublisherEaDistributionTarget> Targets { get; } = new();
        public EaOrderText Text { get; private set; }
        public Dictionary<EaOrderId, EaOrderEntity> OrdersById { get; } = new();
        public Dictionary<EaTicketNumber, EaOrderEntity> OrdersByTicketNumber { get; } = new();

        public PublisherEaAggregate(PublisherEaId id) : base(id)
        {
        }

        public void Register(SecretKey key, DistributionGroupId distributionGroupId, PublisherId publisherId)
        {
            Emit(new PublisherEaRegisteredEvent(key, distributionGroupId, publisherId));
        }

        public void UpdateOrderText(EaOrderText text)
        {
            if (Text != text)
            {
                Emit(new PublisherEaOrderTextUpdatedEvent(Key, text));
                var prevOrderTicketNos = OrdersByTicketNumber.Keys;
                var nextOrders = text.ToOrders().ToHashSet();
                var nextOrderTicketNos = nextOrders.Select(e => e.TicketNo).ToHashSet();

                foreach (var prevTicketNo in prevOrderTicketNos)
                {
                    if (!nextOrderTicketNos.Contains(prevTicketNo))
                    {
                        var order = OrdersByTicketNumber[prevTicketNo];
                        Emit(new PublisherEaClosedOrderEvent(order));
                    }
                }
                foreach (var order in nextOrders)
                {
                    if (!prevOrderTicketNos.Contains(order.TicketNo))
                    {
                        Emit(new PublisherEaOpenedOrderEvent(new EaOrderEntity(EaOrderId.New, order.TicketNo, order.Symbol, order.OrderType, Targets.Select(t => new PublisherEaCopyTradeEntity(CopyTradeId.New, t.DistributionGroupId, t.PublisherId)).ToList())));
                    }
                }
            }
        }

        public void Apply(PublisherEaRegisteredEvent aggregateEvent)
        {
            Key = aggregateEvent.Key;
            Targets.Add(new PublisherEaDistributionTarget(aggregateEvent.DistributionGroupId, aggregateEvent.PublisherId));
        }

        public void Apply(PublisherEaOrderTextUpdatedEvent aggregateEvent)
        {
            Text = aggregateEvent.Text;
        }

        public void Apply(PublisherEaOpenedOrderEvent aggregateEvent)
        {
            var entity = aggregateEvent.Order;
            OrdersByTicketNumber.Add(entity.TicketNo, entity);
            OrdersById.Add(entity.Id, entity);
        }

        public void Apply(PublisherEaClosedOrderEvent aggregateEvent)
        {
            var entity = aggregateEvent.Order;
            OrdersByTicketNumber.Remove(entity.TicketNo);
            OrdersById.Remove(entity.Id);
        }
    }
}