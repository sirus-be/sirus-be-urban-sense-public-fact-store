using FactStore.Api.Handlers;
using FactStore.Api.Infrastructure.Database;
using FactStore.Api.Infrastructure.DataStores;
using FactStore.Api.Services;
using FactStore.Jobs.Services;
using Hangfire;
using Hangfire.Console;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Authentication;

namespace FactStore.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {

        }

        public static void AddDataStores(this IServiceCollection services)
        {
            services.AddTransient<IFactStore, FactStoreStore>();
        }

        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IFactStoreService, FactStoreService>();
        }

        public static void AddBackgroundService(this IServiceCollection services, IConfiguration configuration)
        {
            var jobStorageConnectionString = configuration.GetConnectionString("FactStoreJobsDatabase");
            if (!string.IsNullOrEmpty(jobStorageConnectionString))
            {
                services.AddHangfire(x =>
                {
                    x.UsePostgreSqlStorage(jobStorageConnectionString);
                    x.UseConsole();
                });
            }
            //JobStorage.Current = new PostgreSqlStorage(jobStorageConnectionString);
            
            services.AddTransient<IBackgroundProcessingService, BackgroundProcessingService>();
        }

        public static void AddCustomAuthentication(this IServiceCollection services, bool isDevelopment, IConfiguration configuration)
        {
            var identityProvider = configuration.GetSection("IdentityProvider").Value;
            
            if(identityProvider == "IdentityServer")
            {
                var identityProviderUrl = configuration.GetSection("IdentityProviderUrl").Value;
                var apiName = configuration.GetSection("ApiConfiguration:Name").Value;
                int apiVersion = int.Parse(configuration.GetSection("ApiConfiguration:Version").Value);

                services.AddAuthentication("Bearer")
                    .AddIdentityServerAuthentication("Bearer", options =>
                    {
                        options.ApiName = apiName;
                        options.Authority = $"{identityProviderUrl}";
                        if (isDevelopment)
                        {   // to make sure this is only used during development
                        options.RequireHttpsMetadata = false;
                        }
                        if (identityProviderUrl == configuration["OpenIdAuthentication:Authority"])
                    {
                            options.RequireHttpsMetadata = true;
                            options.JwtBackChannelHandler = GetHandler();
                        }
                    });
            }
            else
            {
                services.AddJwtAuthentication(isDevelopment, options =>
                {
                    options.Audience = configuration["OpenIdAuthentication:Audience"];
                    options.Authority = configuration["OpenIdAuthentication:Authority"];
                    options.TokenValidationParameters.ValidIssuer = options.Authority;
                });
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("ValidRoleRolesPolicy", policy =>
                        policy.Requirements.Add(new ValidRoleRolesRequirement()));

                    options.DefaultPolicy = new AuthorizationPolicyBuilder(
                        JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
                });

                services.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();
            }
        }

        public static void AddDatabases(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEntityFrameworkNpgsql();

            services.AddDbContext<FactStoreDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("FactStoreDatabase"));
            });
        }

        public static void RegisterSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"v{configuration.GetSection("ApiConfiguration:Version").Value}", new OpenApiInfo
                {
                    Title = configuration.GetSection("ApiConfiguration:Name").Value,
                    Version = $"v{configuration.GetSection("ApiConfiguration:Version").Value}",
                    Description = "FactStore API",
                    Contact = new OpenApiContact
                    {
                        Name = "Sirus",
                        Url = new Uri("https://www.sirus.be")
                    }
                });
                c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {Value}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearerAuth"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                var xmlfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlfile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        private static HttpClientHandler GetHandler()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.SslProtocols = SslProtocols.Tls12;
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            return handler;
        }

    }
}
