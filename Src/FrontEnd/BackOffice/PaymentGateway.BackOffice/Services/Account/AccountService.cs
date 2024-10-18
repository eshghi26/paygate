using System.Net.Http.Json;

namespace PaymentGateway.BackOffice.Services.Account
{
    public class AccountService (HttpClient httpClient) : IAccountService
    {
        public async Task<ApiResult<string>> Login(string username, string password, string captchaCode)
        {
            var response = await httpClient.PostAsJsonAsync("account/Login",
                new
                {
                    username,
                    password,
                    captchaCode
                });

            var resultStr = await response.Content.ReadAsStringAsync();

            return new ApiResult<string>(resultStr, response.StatusCode);
        }
    }
}
