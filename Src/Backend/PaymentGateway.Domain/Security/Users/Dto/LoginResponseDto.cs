namespace PaymentGateway.Domain.Security.Users.Dto
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
    }
}
