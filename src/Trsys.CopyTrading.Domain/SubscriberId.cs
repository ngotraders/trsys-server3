﻿using EventFlow.Core;
using EventFlow.ValueObjects;
using System.Text.Json.Serialization;

namespace Trsys.CopyTrading.Domain
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class SubscriberId : Identity<SubscriberId>
    {
        public SubscriberId(string value) : base(value)
        {
        }
    }
}