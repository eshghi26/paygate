namespace PaymentGateway.BackOffice.Services.Account
{
    public interface IAccountService
    {
        Task<ApiResult<string>> Login(string username, string password, string captchaCode);
    }
}
