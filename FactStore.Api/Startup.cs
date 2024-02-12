using FactStore.Api.Extensions;
using FactStore.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace FactStore.Api
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
            services.RegisterConfiguration(Configuration);
            services.AddCustomAuthentication(HostEnvironment.IsDevelopment(), Configuration);
            services.RegisterSwagger(Configuration);
            services.AddDatabases(Configuration);
            services.AddBackgroundService(Configuration);

            services.AddControllers();
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(int.Parse(Configuration.GetSection("ApiConfiguration:Version").Value), 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });

            services.AddHttpContextAccessor();
            services.AddDataStores();
            services.AddApplicationServices();

            services.AddDistributedMemoryCache(config =>
            {
                config.CompactionPercentage = 0.25;
                config.SizeLimit = 1024;
            });

            // Setup cross origins policy
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, FactStoreDbContext dataContext)
        {
            dataContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                var apiName = Configuration.GetSection("ApiConfiguration:Name").Value;
                int apiVersion = int.Parse(Configuration.GetSection("ApiConfiguration:Version").Value);

                app.UseDeveloperExceptionPage();
                app.UseSwagger(c => {
                    c.PreSerializeFilters.Add((swagger, httpReq) =>
                    {
                        swagger.Servers = httpReq.Host.Host.StartsWith("localhost", StringComparison.OrdinalIgnoreCase) ? null : new List<OpenApiServer> { new OpenApiServer { Url = $"https://{httpReq.Host.Host}" } };
                    });
                });
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{apiName} v{ apiVersion}"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
