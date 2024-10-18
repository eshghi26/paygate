namespace PaymentGateway.Domain.Security.Users.Dto
{
    public class LoginRequestDto
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public string? CaptchaCode { get; set; }
    }
}
