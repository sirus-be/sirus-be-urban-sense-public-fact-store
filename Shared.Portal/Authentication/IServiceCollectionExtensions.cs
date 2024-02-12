using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Shared.Portal.Authentication
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            var clientId = configuration.GetSection("OpenIdAuthentication:ClientId").Value;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = async cookieCtx =>
                    {
                        var accessToken = cookieCtx.Properties.GetTokenValue("access_token");
                        cookieCtx.HttpContext.Response.Cookies.Append("access_token", accessToken);

                        var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
                        MapKeycloakRolesToRoleClaims(cookieCtx, token, clientId);
                    }
                };
            })
            .AddOpenIdConnect("oidc", options =>
             {
                 options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                 options.Authority = configuration.GetSection("OpenIdAuthentication:Authority").Value;
                 options.ClientId = clientId;
                 options.ClientSecret = configuration.GetSection("OpenIdAuthentication:ClientSecret").Value;
                 options.ResponseType = "code";
                 options.SaveTokens = true;
                 options.GetClaimsFromUserInfoEndpoint = true;
                 options.RequireHttpsMetadata = !isDevelopment;
                 options.Scope.Add(configuration.GetSection("OpenIdAuthentication:Scope").Value);
                 options.UseTokenLifetime = true;
             });

            services.Configure<OpenIdAuthenticationOptions>(configuration.GetSection("OpenIdAuthentication"));
            services.AddScoped<AuthenticationStateProvider, ServerAuthenticationState>();

            return services;
        }

        private static void MapKeycloakRolesToRoleClaims(CookieValidatePrincipalContext context, JwtSecurityToken token, string clientId)
        {
            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            if (token == null)
            {
                return;
            }
            if (token.Claims.First(claim => claim.Type == "resource_access") != null)
            {
                var resourceAccess = JObject.Parse(token.Claims.First(claim => claim.Type == "resource_access").Value);
                var clientResource = resourceAccess[clientId];
                var clientRoles = clientResource?["roles"];

                if (clientRoles != null)
                {
                    foreach (var clientRole in clientRoles)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, clientRole.ToString()));
                    }
                }
            }

            if (token.Claims.First(claim => claim.Type == "realm_access") != null)
            {
                var realmAccess = JObject.Parse(token.Claims.First(claim => claim.Type == "realm_access").Value);
                var realmRoles = realmAccess?["roles"].ToObject<string[]>();
                if (realmRoles != null)
                {
                    foreach (var realmRole in realmRoles)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, realmRole));
                    }
                }
            }
        }
    }
}
