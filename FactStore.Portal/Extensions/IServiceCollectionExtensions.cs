using Core.Authentication;
using FactStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using System;

namespace FactStore.Portal.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FactStoreServiceClientOptions>(configuration.GetSection("FactStoreService"));
            var factStoreServiceClientOptions = configuration.GetSection("FactStoreService").Get<FactStoreServiceClientOptions>();

            return services.AddFactStoreServiceClient(factStoreServiceClientOptions);
        }

        public static IServiceCollection AddRadzenServices(this IServiceCollection services)
        {
            services.AddScoped<NotificationService>();
            services.AddScoped<DialogService>();
            return services;
        }

        private static IServiceCollection AddFactStoreServiceClient(this IServiceCollection services, FactStoreServiceClientOptions factStoreServiceClientOptions)
        {
            services.AddScoped<TokenProvider>();

            services
                .AddHttpClient<IFactStoreServiceClient, FactStoreServiceClient>(options =>
            {
                options.BaseAddress = new Uri(factStoreServiceClientOptions.Uri);
            });

            return services;
        }
    }
}
