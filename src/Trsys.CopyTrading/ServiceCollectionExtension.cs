﻿using Microsoft.Extensions.DependencyInjection;

namespace Trsys.CopyTrading
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCopyTrading(this IServiceCollection services)
        {
            return services;
        }
    }
}
