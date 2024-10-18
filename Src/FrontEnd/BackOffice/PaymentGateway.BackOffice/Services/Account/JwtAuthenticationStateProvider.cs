using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PaymentGateway.BackOffice.Services.Account
{
    public class JwtAuthenticationStateProvider(ILocalStorageService localStorageService)
        : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await localStorageService.GetItemAsync<string>("authToken");

                if (string.IsNullOrEmpty(token))
                {
                    return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
                }

                var claims = GetClaimsFromToken(token);
                var identity = new ClaimsIdentity(claims, "JwtAuth");
                var user = new ClaimsPrincipal(identity);

                return await Task.FromResult(new AuthenticationState(user));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            }
        }

        public async Task UpdateAuthenticationState(string? token)
        {
            ClaimsPrincipal claimsPrincipal;
            if (!string.IsNullOrWhiteSpace(token))
            {
                var userSession = GetClaimsFromToken(token);
                var identity = new ClaimsIdentity(userSession, "JwtAuth");
                claimsPrincipal = new ClaimsPrincipal(identity);

                await localStorageService.SetItemAsStringAsync("authToken", token);
            }
            else
            {
                claimsPrincipal = new(new ClaimsIdentity());
                await localStorageService.RemoveItemAsync("authToken");
            }

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        private IEnumerable<Claim> GetClaimsFromToken(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return token.Claims;
        }
    }
}
