﻿using EventFlow.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trsys.CopyTrading.Domain
{
    public class CopyTradeAggregate : AggregateRoot<CopyTradeAggregate, CopyTradeId>,
        IEmit<CopyTradeOpenedEvent>,
        IEmit<CopyTradeApplicantAddedEvent>,
        IEmit<CopyTradeClosedEvent>
    {
        public bool IsOpen { get; private set; }
        public HashSet<AccountId> TradeApplicants { get; } = new();

        public CopyTradeAggregate(CopyTradeId id) : base(id)
        {
        }

        public void Open(DistributionGroupId distributionGroupId, ForexTradeSymbol symbol, OrderType orderType)
        {
            if (!IsNew)
            {
                throw new InvalidOperationException();
            }
            Emit(new CopyTradeOpenedEvent(distributionGroupId, symbol, orderType), new Metadata(KeyValuePair.Create("copy-trade-id", Id.Value)));
        }

        public void AddApplicant(AccountId accountId)
        {
            if (IsNew || !IsOpen)
            {
                throw new InvalidOperationException();
            }
            Emit(new CopyTradeApplicantAddedEvent(accountId));
        }

        public void Close()
        {
            if (IsNew)
            {
                throw new InvalidOperationException();
            }
            if (IsOpen)
            {
                Emit(new CopyTradeClosedEvent(TradeApplicants.ToArray()), new Metadata(KeyValuePair.Create("copy-trade-id", Id.Value)));
            }
        }

        public void Apply(CopyTradeOpenedEvent _)
        {
            IsOpen = true;
        }

        public void Apply(CopyTradeApplicantAddedEvent e)
        {
            TradeApplicants.Add(e.AccountId);
        }

        public void Apply(CopyTradeClosedEvent _)
        {
            IsOpen = false;
        }
    }
}
