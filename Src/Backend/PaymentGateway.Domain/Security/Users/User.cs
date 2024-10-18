using System.Text.Json.Serialization;

namespace PaymentGateway.Domain.Security.Users
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public string? Password { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? NickName { get; set; }
        public required string Email { get; set; }
        public string? MobileNumber { get; set; }
        public string? ProfilePicture { get; set; }
        public bool IsActive { get; set; }
        public bool IsBan { get; set; }
        public DateTime CreateOn { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public string? RefreshTokenSerial { get; set; }

        [JsonIgnore]
        public DateTime? RefreshTokenExpiryDate { get; set; }
    }
}
