using Microsoft.AspNetCore.Mvc.Rendering;

namespace PaymentGateway.GatewayUi.Models
{
    public class PaymentViewModel
    {
        public decimal Amount { get; set; }
        public string? Pan { get; set; }
        public string? Cvv2 { get; set; }
        public string? ExpYear { get; set; }
        public string? ExpMonth { get; set; }
        public double ExpireTime { get; set; }
        public bool SavePan { get; set; }
        public string? CaptchaAddress { get; set; }
        public string? CaptchaCode { get; set; }
        public string? Token { get; set; }

        public List<SelectListItem>? Cards { get; set; }
    }
}