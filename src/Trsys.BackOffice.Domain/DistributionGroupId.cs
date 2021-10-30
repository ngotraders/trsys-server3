﻿using EventFlow.Core;
using EventFlow.ValueObjects;
using System.Text.Json.Serialization;

namespace Trsys.BackOffice.Domain
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class DistributionGroupId : Identity<DistributionGroupId>
    {
        public DistributionGroupId(string value) : base(value)
        {
        }
    }
}