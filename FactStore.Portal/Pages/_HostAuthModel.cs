using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace FactStore.Portal.Pages
{
    public class _HostAuthModel: PageModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (accessToken != null)
            {
                var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

                if (token.ValidTo < DateTimeOffset.UtcNow.AddMinutes(1))
                {
                    RefreshToken = null;
                    AccessToken = null;
                    await HttpContext.SignOutAsync("Cookies");
                    await HttpContext.SignOutAsync("oidc");
                }
                else
                {
                    AccessToken = accessToken;
                    RefreshToken = await HttpContext.GetTokenAsync("refresh_token");
                }
            }

            return Page();
        }
    }
}
