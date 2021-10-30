﻿using EventFlow;
using EventFlow.Queries;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trsys.CopyTrading;
using Trsys.CopyTrading.Abstractions;
using Trsys.Ea.Application.Read.Models;
using Trsys.Ea.Application.Write.Commands;
using Trsys.Ea.Domain;

namespace Trsys.Ea
{
    public class CopyTradingEventHandler : BackgroundService
    {
        private readonly ICopyTradingService service;
        private readonly ICommandBus commandBus;
        private readonly IQueryProcessor queryProcessor;

        public CopyTradingEventHandler(ICopyTradingService service, ICommandBus commandBus, IQueryProcessor queryProcessor)
        {
            this.service = service;
            this.commandBus = commandBus;
            this.queryProcessor = queryProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await service.SubscribeToCopyTradeEventsAsync(OnCopyTradeEvent, stoppingToken);
                }
                catch
                {
                }
            }
        }

        private Task OnCopyTradeEvent(ICopyTradingEvent copyTradingEvent)
        {
            if (copyTradingEvent is CopyTradeOpened opened)
            {
                return HandleAsync(opened, CancellationToken.None);
            }
            if (copyTradingEvent is CopyTradeClosed closed)
            {
                return HandleAsync(closed, CancellationToken.None);
            }
            return Task.CompletedTask;
        }

        public async Task HandleAsync(CopyTradeOpened domainEvent, CancellationToken cancellationToken)
        {
            await Task.WhenAll(domainEvent.Subscribers.Select(async subscriberId =>
            {
                var subscriber = await queryProcessor.ProcessAsync(new ReadModelByIdQuery<SubscriberIdToSubscriberEaIdReadModel>(subscriberId), cancellationToken);
                if (subscriber == null)
                {
                    return;
                }
                await commandBus.PublishAsync(new SubscriberEaOpenTradeOrderCommand(SubscriberEaId.With(subscriber.SubscriberEaId), CopyTradeId.With(domainEvent.CopyTradeId), new ForexTradeSymbol(domainEvent.Symbol), OrderType.Of(domainEvent.OrderType)), cancellationToken);
            }));
        }

        public async Task HandleAsync(CopyTradeClosed domainEvent, CancellationToken cancellationToken)
        {
            await Task.WhenAll(domainEvent.Subscribers.Select(async subscriberId =>
            {
                var subscriber = await queryProcessor.ProcessAsync(new ReadModelByIdQuery<SubscriberIdToSubscriberEaIdReadModel>(subscriberId), cancellationToken);
                if (subscriber == null)
                {
                    return;
                }
                await commandBus.PublishAsync(new SubscriberEaCloseTradeOrderCommand(SubscriberEaId.With(subscriber.SubscriberEaId), CopyTradeId.With(domainEvent.CopyTradeId)), cancellationToken);
            }));
        }

    }
}