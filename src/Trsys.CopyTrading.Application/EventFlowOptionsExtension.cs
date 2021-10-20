﻿using EventFlow;
using EventFlow.Extensions;
using System.Collections.Generic;
using Trsys.CopyTrading.Application.Read.Models;
using Trsys.CopyTrading.Application.Read.Queries;
using Trsys.CopyTrading.Application.Write.Commands;
using Trsys.CopyTrading.Application.Write.Sagas.Ea;
using Trsys.CopyTrading.Application.Write.Sagas.TradeDistribution;
using Trsys.CopyTrading.Application.Write.Subscribers;
using Trsys.CopyTrading.Domain;

namespace Trsys.CopyTrading.Application
{
    public static class EventFlowOptionsExtension
    {
        public static IEventFlowOptions UseApplication(this IEventFlowOptions options)
        {
            options
                .AddCommands(
                    typeof(AccountStateUpdateCommand),
                    typeof(AccountRequestOpenTradeOrderCommand),
                    typeof(AccountRequestCloseTradeOrderCommand),
                    typeof(AccountDistributeOpenTradeOrderRequestCommand),
                    typeof(AccountDistributeCloseTradeOrderRequestCommand),
                    typeof(DistributionGroupAddPublisherCommand),
                    typeof(DistributionGroupAddSubscriberCommand),
                    typeof(DistributionGroupPublishOpenCommand),
                    typeof(DistributionGroupPublishCloseCommand),
                    typeof(CopyTradeOpenCommand),
                    typeof(CopyTradeAddDistributedAccountCommand),
                    typeof(CopyTradeCloseCommand),
                    typeof(CopyTradeRemoveDistributedAccountCommand),
                    typeof(PublisherEaRegisterCommand),
                    typeof(PublisherEaUnregisterCommand),
                    typeof(PublisherEaUpdateOrderTextCommand),
                    typeof(SubscriberEaRegisterCommand),
                    typeof(SubscriberEaUnregisterCommand),
                    typeof(SubscriberEaOpenTradeOrderCommand),
                    typeof(SubscriberEaCloseTradeOrderCommand),
                    typeof(SubscriberEaDistributeOrderTextCommand)
                )
                .AddCommandHandlers(
                    typeof(AccountStateUpdateCommandHandler),
                    typeof(AccountRequestOpenTradeOrderCommandHandler),
                    typeof(AccountRequestCloseTradeOrderCommandHandler),
                    typeof(AccountDistributeOpenTradeOrderRequestCommandHandler),
                    typeof(AccountDistributeCloseTradeOrderRequestCommandHandler),
                    typeof(DistributionGroupAddPublisherCommandHandler),
                    typeof(DistributionGroupAddSubscriberCommandHandler),
                    typeof(DistributionGroupPublishOpenCommandHandler),
                    typeof(DistributionGroupPublishCloseCommandHandler),
                    typeof(CopyTradeOpenCommandHandler),
                    typeof(CopyTradeCloseCommandHandler),
                    typeof(CopyTradeAddDistributedAccountCommandHandler),
                    typeof(CopyTradeRemoveDistributedAccountCommandHandler),
                    typeof(PublisherEaRegisterCommandHandler),
                    typeof(PublisherEaUnregisterCommandHandler),
                    typeof(PublisherEaUpdateOrderTextCommandHandler),
                    typeof(SubscriberEaRegisterCommandHandler),
                    typeof(SubscriberEaUnregisterCommandHandler),
                    typeof(SubscriberEaOpenTradeOrderCommandHandler),
                    typeof(SubscriberEaCloseTradeOrderCommandHandler),
                    typeof(SubscriberEaDistributeOrderTextCommandHandler)
                )
                .AddEvents(
                    typeof(AccountStateUpdatedEvent),
                    typeof(AccountTradeOrderOpenRequestedEvent),
                    typeof(AccountTradeOrderOpenRequestDistributedEvent),
                    typeof(AccountTradeOrderCloseRequestedEvent),
                    typeof(AccountTradeOrderCloseRequestDistributedEvent),
                    typeof(AccountTradeOrderInactivatedEvent),
                    typeof(DistributionGroupPublisherAddedEvent),
                    typeof(DistributionGroupSubscriberAddedEvent),
                    typeof(DistributionGroupPublishedOpenEvent),
                    typeof(DistributionGroupPublishedCloseEvent),
                    typeof(CopyTradeOpenedEvent),
                    typeof(CopyTradeApplicantAddedEvent),
                    typeof(CopyTradeClosedEvent),
                    typeof(CopyTradeApplicantRemovedEvent),
                    typeof(CopyTradeFinishedEvent),
                    typeof(PublisherEaRegisteredEvent),
                    typeof(PublisherEaUnregisteredEvent),
                    typeof(PublisherEaOrderTextChangedEvent),
                    typeof(PublisherEaOpenedOrderEvent),
                    typeof(PublisherEaClosedOrderEvent),
                    typeof(SubscriberEaRegisteredEvent),
                    typeof(SubscriberEaUnregisteredEvent),
                    typeof(SubscriberEaTradeOrderOpenRequestAppliedEvent),
                    typeof(SubscriberEaTradeOrderCloseRequestAppliedEvent),
                    typeof(SubscriberEaOrderTextChangedEvent),
                    typeof(SubscriberEaDistributedOrderTextChangedEvent)
                )
                .AddSubscribers(
                    typeof(AccountTradeOrderRequestEventSubscriber)
                )
                .AddSagaLocators(
                    typeof(TradeDistributionSagaLocator),
                    typeof(PublisherEaRegistrationSagaLocator),
                    typeof(SubscriberEaRegistrationSagaLocator),
                    typeof(OrderPublishingSagaLocator)
                )
                .AddSagas(
                    typeof(TradeDistributionSaga),
                    typeof(PublisherEaRegistrationSaga),
                    typeof(SubscriberEaRegistrationSaga),
                    typeof(OrderPublishingSaga)
                )
                .AddEvents(
                    typeof(TradeDistributionSagaStartedEvent),
                    typeof(TradeDistributionSagaFinishedEvent)
                );
            options
                .RegisterServices(sr => {
                    sr.RegisterType(typeof(CopyTradeReadModelLocator));
                    sr.RegisterType(typeof(PublisherEaReadModelLocator));
                    sr.RegisterType(typeof(SubscriberEaReadModelLocator));
                    sr.RegisterType(typeof(AccountIdToSubscriberEaIdReadModelLocator));
                })
                .UseInMemoryReadStoreFor<AccountReadModel>()
                .UseInMemoryReadStoreFor<DistributionGroupReadModel>()
                .UseInMemoryReadStoreFor<CopyTradeReadModel, CopyTradeReadModelLocator>()
                .UseInMemoryReadStoreFor<PublisherEaReadModel, PublisherEaReadModelLocator>()
                .UseInMemoryReadStoreFor<SubscriberEaReadModel, SubscriberEaReadModelLocator>()
                .UseInMemoryReadStoreFor<AccountIdToSubscriberEaIdReadModel, AccountIdToSubscriberEaIdReadModelLocator>()
                .AddQueryHandler<CopyTradeReadModelAllQueryHandler, CopyTradeReadModelAllQuery, IReadOnlyCollection<CopyTradeReadModel>>();
            return options;
        }
    }
}
