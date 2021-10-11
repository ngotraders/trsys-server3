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
    public class PublishOrderCloseCommandTests
    {
        [TestMethod]
        public async Task SuccessWithoutSubscriber()
        {
            using var resolver = CreateResolver();
            var commandBus = resolver.Resolve<ICommandBus>();
            var copyTradeId = CopyTradeId.New;
            var publisherId = PublisherId.New;
            var distributionGroupId = DistributionGroupId.New;
            var result = await commandBus.PublishAsync(new PublishOrderOpenCommand(copyTradeId, publisherId, distributionGroupId, ForexTradeSymbol.ValueOf("USDJPY"), OrderType.Buy), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);
            result = await commandBus.PublishAsync(new PublishOrderCloseCommand(copyTradeId, publisherId), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);

            var queryProcessor = resolver.Resolve<IQueryProcessor>();
            var queryResult = await queryProcessor.ProcessAsync(new ReadModelByIdQuery<CopyTradeReadModel>(copyTradeId), CancellationToken.None);
            Assert.AreEqual(distributionGroupId.Value, queryResult.DistributionGroupId);
            Assert.AreEqual("USDJPY", queryResult.Symbol);
            Assert.AreEqual("BUY", queryResult.OrderType);
            Assert.IsFalse(queryResult.IsOpen);
        }

        [TestMethod]
        public async Task SuccessWithASubscriber()
        {
            using var resolver = CreateResolver();
            var commandBus = resolver.Resolve<ICommandBus>();

            var accountId = AccountId.New;
            var publisherId = PublisherId.New;
            var distributionGroupId = DistributionGroupId.New;
            var result = await commandBus.PublishAsync(new AddSubscriberCommand(distributionGroupId, accountId), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);
            var copyTradeId = CopyTradeId.New;
            result = await commandBus.PublishAsync(new PublishOrderOpenCommand(copyTradeId, publisherId, distributionGroupId, ForexTradeSymbol.ValueOf("USDJPY"), OrderType.Buy), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);
            result = await commandBus.PublishAsync(new PublishOrderCloseCommand(copyTradeId, publisherId), CancellationToken.None);
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
            foreach (var accountId in Enumerable
                .Range(0, 100)
                .Select(_ => AccountId.New))
            {
                result = await commandBus.PublishAsync(new AddSubscriberCommand(distributionGroupId, accountId), CancellationToken.None);
                Assert.IsTrue(result.IsSuccess);
            }
            var copyTradeId = CopyTradeId.New;
            result = await commandBus.PublishAsync(new PublishOrderOpenCommand(copyTradeId, publisherId, distributionGroupId, ForexTradeSymbol.ValueOf("USDJPY"), OrderType.Buy), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);
            result = await commandBus.PublishAsync(new PublishOrderCloseCommand(copyTradeId, publisherId), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);

            var queryProcessor = resolver.Resolve<IQueryProcessor>();
            var queryResult = await queryProcessor.ProcessAsync(new ReadModelByIdQuery<CopyTradeReadModel>(copyTradeId), CancellationToken.None);
            Assert.AreEqual(distributionGroupId.Value, queryResult.DistributionGroupId);
            Assert.AreEqual("USDJPY", queryResult.Symbol);
            Assert.AreEqual("BUY", queryResult.OrderType);
            Assert.IsFalse(queryResult.IsOpen);
        }

        private static IRootResolver CreateResolver()
        {
            return EventFlowOptions
                .New
                .UseApplication()
                .CreateResolver();
        }
    }
}
