using EventFlow;
using EventFlow.Configuration;
using EventFlow.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using Trsys.CopyTrading.Application;
using Trsys.CopyTrading.Application.Read.Models;
using Trsys.CopyTrading.Domain;
using Trsys.Ea.Application.Write.Commands;
using Trsys.Ea.Domain;

namespace Trsys.Ea.Application.Tests
{
    [TestClass]
    public class SubscriberEaRegisterCommandTests
    {
        [TestMethod]
        public async Task Success()
        {
            using var resolver = CreateResolver();
            var commandBus = resolver.Resolve<ICommandBus>();

            var subscriberEaId = SubscriberEaId.New;

            var distributionGroupId = DistributionGroupId.New;
            var subscriberId = AccountId.New;

            var result = await commandBus.PublishAsync(new SubscriberEaRegisterCommand(subscriberEaId, new SecretKey("SubscriberKey"), distributionGroupId, subscriberId), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);

            var queryProcessor = resolver.Resolve<IQueryProcessor>();
            var queryResult = await queryProcessor.ProcessAsync(new ReadModelByIdQuery<DistributionGroupReadModel>(distributionGroupId.Value), CancellationToken.None);
            Assert.IsTrue(queryResult.Subscribers.Contains(subscriberId.Value));
        }

        private static IRootResolver CreateResolver()
        {
            return EventFlowOptions
                .New
                .UseCopyTradeApplication()
                .UseEaApplication()
                .CreateResolver();
        }
    }
}