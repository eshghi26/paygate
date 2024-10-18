using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace PaymentGateway.BackOffice.Services.Account
{
    public class JwtHttpClientHandler(ILocalStorageService localStorageService)
        : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await localStorageService.GetItemAsync<string>("authToken", cancellationToken);

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
