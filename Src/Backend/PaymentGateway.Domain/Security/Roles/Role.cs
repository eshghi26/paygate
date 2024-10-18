namespace PaymentGateway.Domain.Security.Roles
{
    public class Role
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Comment { get; set; }
        public DateTime CreateOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
