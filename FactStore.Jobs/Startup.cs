using FactStore.Jobs.Tasks;
using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FactStore.Jobs
{
    public class Startup
    {
        public IWebHostEnvironment HostEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var clientId = Configuration.GetSection("OpenIdAuthentication:ClientId").Value;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie()
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = Configuration.GetSection("OpenIdAuthentication:Authority").Value;
                options.ClientId = clientId;
                options.ClientSecret = Configuration.GetSection("OpenIdAuthentication:ClientSecret").Value;
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = true;
                options.Scope.Add(Configuration.GetSection("OpenIdAuthentication:Scope").Value);
                options.UseTokenLifetime = true;
            });

            services.AddHangfire(x => x
                        .UseConsole()
                        .UseRecommendedSerializerSettings()
                        .UsePostgreSqlStorage(Configuration.GetConnectionString("FactStoreJobsDatabase")));
            services.AddHangfireConsoleExtensions();
            services.AddHangfireServer();

            services.AddTransient<IUpdateFactValueTask, UpdateFactValueTask>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Equals("/hangfire", StringComparison.OrdinalIgnoreCase)
                    && !context.User.Identity.IsAuthenticated)
                {
                    await context.ChallengeAsync();
                    return;
                }

                await next();
            });


            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new IDashboardAuthorizationFilter[]
                {
                    new DashboardAuthorizationFilter()
                }
            });
        }
    }
}
