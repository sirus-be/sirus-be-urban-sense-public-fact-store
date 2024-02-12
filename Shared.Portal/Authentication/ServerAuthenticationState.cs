using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Net.Http;
using IdentityModel.Client;
using System.IdentityModel.Tokens.Jwt;
using Core.Authentication;
using Microsoft.Extensions.Options;

namespace Shared.Portal.Authentication
{
    public class ServerAuthenticationState : RevalidatingServerAuthenticationStateProvider
    {
        private readonly TokenProvider _tokenProvider;
        private readonly OpenIdAuthenticationOptions _openIdConnectOptions;

        public ServerAuthenticationState(TokenProvider tokenProvider, IOptions<OpenIdAuthenticationOptions> openIdConnectOptions, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _tokenProvider = tokenProvider;
            _openIdConnectOptions = openIdConnectOptions.Value;
        }

        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(1);

        protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            var tokenData = _tokenProvider.Get();
            if (tokenData.AccessToken != null)
            {
                var accessToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenData.AccessToken);

                if (accessToken.ValidTo < DateTimeOffset.UtcNow.AddMinutes(1) && tokenData.RefreshToken != null)
                {
                    var refreshToken = tokenData.RefreshToken;

                    var response = await new HttpClient().RequestRefreshTokenAsync(new RefreshTokenRequest
                    {
                        Address = $"{_openIdConnectOptions.Authority}/protocol/openid-connect/token",
                        ClientId = _openIdConnectOptions.ClientId,
                        ClientSecret = _openIdConnectOptions.ClientSecret,
                        RefreshToken = refreshToken,
                        GrantType = "refresh_token"
                    });

                    if (!response.IsError)
                    {
                        _tokenProvider.Set(response.AccessToken, response.RefreshToken);

                        return true;
                    }
                    else
                    {
                        _tokenProvider.Remove();

                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }
}
