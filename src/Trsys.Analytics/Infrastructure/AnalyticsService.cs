﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Trsys.Analytics.Abstractions;

namespace Trsys.Analytics.Infrastructure
{
    internal class AnalyticsService : IAnalyticsService
    {
        public Task<CopyTradeDto> FindCopyTradeByIdAsync(string copyTradeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task OpenCopyTradeAsync(string copyTradeId, DateTimeOffset dateTimeOffset, string symbol, string orderType)
        {
            throw new NotImplementedException();
        }
    }
}