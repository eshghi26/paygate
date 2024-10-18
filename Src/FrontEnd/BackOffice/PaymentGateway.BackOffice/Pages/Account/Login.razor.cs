using Microsoft.IdentityModel.Tokens;
using PaymentGateway.BackOffice.Services.Account;
using Radzen;
using System.Web;
using PaymentGateway.BackOffice.Services;

namespace PaymentGateway.BackOffice.Pages.Account
{
    public partial class Login
    {
        #region Data Members
        string? _returnUrl;
        #endregion

        private async Task OnLogin(LoginArgs args)
        {
            var loginResult = await AccountService.Login(args.Username, args.Password, "123");

            if (loginResult.Status == ContentStatus.OtherError)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Login failed",
                    Detail = loginResult.Message,
                    Duration = 4000
                });

                return;
            }
            
            var token = loginResult.Data;

            if (!string.IsNullOrWhiteSpace(token))
            {
                var authStateProvider = (JwtAuthenticationStateProvider)AuthStateProvider;
                await authStateProvider.UpdateAuthenticationState(token);

                var absoluteUri = new Uri(NavigationManager.Uri);
                var queryParam = HttpUtility.ParseQueryString(absoluteUri.Query);
                _returnUrl = queryParam["returnUrl"];
                if (string.IsNullOrEmpty(_returnUrl))
                {
                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    NavigationManager.NavigateTo("/" + _returnUrl);
                }
            }

            //toast;
        }
    }
}
