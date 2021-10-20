using EventFlow;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Configuration;
using EventFlow.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trsys.CopyTrading.Application.Read.Models;
using Trsys.CopyTrading.Application.Write.Commands;
using Trsys.CopyTrading.Domain;

namespace Trsys.CopyTrading.Application.Tests
{
    [TestClass]
    public class AccountDistributeCloseTradeOrderRequestCommandTests
    {
        [TestMethod]
        public async Task SuccessWithASubscriber()
        {
            using var resolver = CreateResolver();
            var commandBus = resolver.Resolve<ICommandBus>();

            var publisherId = PublisherId.New;
            var distributionGroupId = DistributionGroupId.New;
            var result = await commandBus.PublishAsync(new DistributionGroupAddPublisherCommand(distributionGroupId, publisherId), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);
            var accountId = AccountId.New;
            result = await commandBus.PublishAsync(new DistributionGroupAddSubscriberCommand(distributionGroupId, accountId), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);
            var copyTradeId = CopyTradeId.New;
            result = await commandBus.PublishAsync(new DistributionGroupPublishOpenCommand(distributionGroupId, publisherId, copyTradeId, ForexTradeSymbol.ValueOf("USDJPY"), OrderType.Buy), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);
            result = await commandBus.PublishAsync(new AccountDistributeOpenTradeOrderRequestCommand(accountId, copyTradeId), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);
            result = await commandBus.PublishAsync(new DistributionGroupPublishCloseCommand(distributionGroupId, publisherId, copyTradeId), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);
            result = await commandBus.PublishAsync(new AccountDistributeCloseTradeOrderRequestCommand(accountId, copyTradeId), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);

            var queryProcessor = resolver.Resolve<IQueryProcessor>();
            var queryResult = await queryProcessor.ProcessAsync(new ReadModelByIdQuery<CopyTradeReadModel>(copyTradeId), CancellationToken.None);
            Assert.AreEqual(distributionGroupId.Value, queryResult.DistributionGroupId);
            Assert.AreEqual("USDJPY", queryResult.Symbol);
            Assert.AreEqual("BUY", queryResult.OrderType);
            Assert.IsFalse(queryResult.IsOpen);
        }

        [TestMethod]
        public async Task SuccessWithMultipleSubscribers()
        {
            var result = default(IExecutionResult);
            using var resolver = CreateResolver();
            var commandBus = resolver.Resolve<ICommandBus>();

            var publisherId = PublisherId.New;
            var distributionGroupId = DistributionGroupId.New;
            result = await commandBus.PublishAsync(new DistributionGroupAddPublisherCommand(distributionGroupId, publisherId), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);
            var accounts = Enumerable.Range(0, 100).Select(_ => AccountId.New).ToArray();
            foreach (var accountId in accounts)
            {
                result = await commandBus.PublishAsync(new DistributionGroupAddSubscriberCommand(distributionGroupId, accountId), CancellationToken.None);
                Assert.IsTrue(result.IsSuccess);
            }
            var copyTradeId = CopyTradeId.New;
            result = await commandBus.PublishAsync(new DistributionGroupPublishOpenCommand(distributionGroupId, publisherId, copyTradeId, ForexTradeSymbol.ValueOf("USDJPY"), OrderType.Buy), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);

            foreach (var accountId in accounts)
            {
                result = await commandBus.PublishAsync(new AccountDistributeOpenTradeOrderRequestCommand(accountId, copyTradeId), CancellationToken.None);
                Assert.IsTrue(result.IsSuccess);
            }
            foreach (var accountId in accounts)
            {
                result = await commandBus.PublishAsync(new AccountDistributeCloseTradeOrderRequestCommand(accountId, copyTradeId), CancellationToken.None);
                Assert.IsTrue(result.IsSuccess);
            }

            var queryProcessor = resolver.Resolve<IQueryProcessor>();
            var queryResult = await queryProcessor.ProcessAsync(new ReadModelByIdQuery<CopyTradeReadModel>(copyTradeId), CancellationToken.None);
            Assert.AreEqual(distributionGroupId.Value, queryResult.DistributionGroupId);
            Assert.AreEqual("USDJPY", queryResult.Symbol);
            Assert.AreEqual("BUY", queryResult.OrderType);
            Assert.IsTrue(queryResult.IsOpen);
            Assert.AreEqual(100, queryResult.TradeOrders.Count);
            Assert.IsTrue(queryResult.TradeOrders.All(e => e.CloseDistributedTimestamp.HasValue));
        }

        private static IRootResolver CreateResolver()
        {
            return EventFlowOptions
                .New
                .UseCopyTradeApplication()
                .CreateResolver();
        }
    }
}