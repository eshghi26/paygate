using Microsoft.AspNetCore.Mvc;
using PaymentGateway.GatewayUi.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PaymentGateway.GatewayUi.Controllers
{
    public class HomeController(IHttpClientFactory httpClientFactory)
        : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
