using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FactStore.Api.Extensions
{
    public static class JwtConfigurationExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services, bool isDevelopment,
            Action<JwtBearerOptions> configureOptions)
        {
            services.AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
                .AddJwtBearer(options =>
                {
                    ConfigureDefaultJwtAuthentication(options, isDevelopment);
                    configureOptions.Invoke(options);
                });
        }

        private static void ConfigureDefaultJwtAuthentication(JwtBearerOptions options, bool isDevelopment)
        {
            if (isDevelopment)
            {
                options.RequireHttpsMetadata = false;
                options.IncludeErrorDetails = true;
            }

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateLifetime = true
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    MapKeycloakRolesToRoleClaims(context, options.Audience);
                    return Task.CompletedTask;
                }
            };
        }

        public static void AddJwtAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });
        }

        private static void MapKeycloakRolesToRoleClaims(TokenValidatedContext context, string audience)
        {
            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                return;
            }

            if (context.Principal.FindFirst("resource_access") != null)
            {
                var resourceAccess = JObject.Parse(context.Principal.FindFirst("resource_access").Value);
                var clientResource = resourceAccess[audience];
                var clientRoles = clientResource?["roles"];
                

                if (clientRoles != null)
                {
                    foreach (var clientRole in clientRoles)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, clientRole.ToString()));
                    }
                }
            }

            if (context.Principal.FindFirst("realm_access") != null)
            {
                var realmAccess = JObject.Parse(context.Principal.FindFirst("realm_access").Value);
                var realmRoles = realmAccess?["roles"];

                if (realmRoles != null)
                {
                    foreach (var realmRole in realmRoles)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, realmRole.ToString()));
                    }
                }
            }
        }
    }
}
