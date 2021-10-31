using EventFlow;
using EventFlow.AspNetCore.Extensions;
using EventFlow.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Trsys.CopyTrading.Abstractions;
using Trsys.Ea.Application;

namespace Trsys.Ea.Infrastructure
{
    public class EaEventFlowRootResolver : IDisposable
    {
        private readonly IRootResolver resolver;
        private bool disposedValue;

        public EaEventFlowRootResolver(IServiceProvider sp)
        {
            resolver = EventFlowOptions
                .New
                .UseEaApplication()
                .RegisterServices(sr =>
                {
                    sr.Register(context => sp.GetRequiredService<ICopyTradingService>());
                })
                .AddAspNetCore()
                .CreateResolver();
        }

        public T Resolve<T>()
        {
            return resolver.Resolve<T>();
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    resolver.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}