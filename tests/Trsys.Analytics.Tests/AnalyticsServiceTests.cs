using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trsys.Analytics.Abstractions;
using Trsys.Core;

namespace Trsys.Analytics.Tests
{
    [TestClass]
    public class AnalyticsServiceTests
    {
        [TestMethod]
        public async Task OpenCopyTradeAsync_Success()
        {
            using var services = new ServiceCollection().AddAnalytics().BuildServiceProvider();
            var service = services.GetRequiredService<IAnalyticsService>();
            var copyTradeId = CopyTradeId.New.Value;
            await service.OpenCopyTradeAsync(copyTradeId, DateTimeOffset.Parse("2021-11-05T01:58:00.000Z"), "USDJPY", "BUY", CancellationToken.None);
            var copyTrade = await service.FindCopyTradeByIdAsync(copyTradeId, CancellationToken.None);
            Assert.AreEqual(copyTradeId, copyTrade.Id);
            Assert.AreEqual(DateTimeOffset.Parse("2021-11-05T01:58:00.000Z"), copyTrade.Duration.OpenedAt);
            Assert.AreEqual("USDJPY", copyTrade.Symbol);
            Assert.AreEqual("BUY", copyTrade.OrderType);
        }

        [TestMethod]
        public async Task CloseCopyTradeAsync_Success()
        {
            using var services = new ServiceCollection().AddAnalytics().BuildServiceProvider();
            var service = services.GetRequiredService<IAnalyticsService>();
            var copyTradeId = CopyTradeId.New.Value;
            await service.CloseCopyTradeAsync(copyTradeId, DateTimeOffset.Parse("2021-11-05T01:58:00.000Z"), CancellationToken.None);
            var copyTrade = await service.FindCopyTradeByIdAsync(copyTradeId, CancellationToken.None);
            Assert.AreEqual(copyTradeId, copyTrade.Id);
            Assert.AreEqual(DateTimeOffset.Parse("2021-11-05T01:58:00.000Z"), copyTrade.Duration.ClosedAt);
        }

        [TestMethod]
        public async Task PublisherOpenedCopyTradeAsync_Success()
        {
            using var services = new ServiceCollection().AddAnalytics().BuildServiceProvider();
            var service = services.GetRequiredService<IAnalyticsService>();
            var publisherId = PublisherId.New.Value;
            var copyTradeId = CopyTradeId.New.Value;
            await service.PublisherOpenCopyTradeAsync(publisherId, copyTradeId, DateTimeOffset.Parse("2021-11-05T01:58:00.000Z"), "USDJPY", "BUY", 101, 102, CancellationToken.None);
            var copyTrade = await service.FindCopyTradeByIdAsync(copyTradeId, CancellationToken.None);
            Assert.AreEqual(copyTradeId, copyTrade.Id);
            Assert.AreEqual(publisherId, copyTrade.PublisherTradeResult.Id);
            Assert.AreEqual(DateTimeOffset.Parse("2021-11-05T01:58:00.000Z"), copyTrade.PublisherTradeResult.Duration.OpenedAt);
            Assert.AreEqual("BUY", copyTrade.PublisherTradeResult.Score.OrderType);
            Assert.AreEqual(101, copyTrade.PublisherTradeResult.Score.PriceOpened);
        }
    }
}
