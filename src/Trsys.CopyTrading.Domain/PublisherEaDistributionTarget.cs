﻿using EventFlow.ValueObjects;
using System.Collections.Generic;

namespace Trsys.CopyTrading.Domain
{
    public class PublisherEaDistributionTarget : ValueObject
    {
        public PublisherEaDistributionTarget(DistributionGroupId distributionGroupId, PublisherId publisherId)
        {
            DistributionGroupId = distributionGroupId;
            PublisherId = publisherId;
        }

        public DistributionGroupId DistributionGroupId { get; }
        public PublisherId PublisherId { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DistributionGroupId;
            yield return PublisherId;
        }
    }
}
