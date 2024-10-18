namespace PaymentGateway.Domain.Security.PermissionTree
{
    public class PermissionTree
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public required string Title { get; set; }
        public int? ControllerId { get; set; }
        public short? ActionType { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
