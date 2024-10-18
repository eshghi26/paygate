namespace PaymentGateway.Domain.Security.Permissions
{
    public class Permission
    {
        public int Id { get; set; }
        public int? RoleId { get; set; }
        public int? UserId { get; set; }
        public int ControllerId { get; set; }
        public short ActionType { get; set; }
        public short AccessType { get; set; }
        public DateTime CreateOn { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}
