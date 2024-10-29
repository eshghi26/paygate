using Microsoft.AspNetCore.Mvc;

namespace PaymentGateway.GatewayUi.Controllers
{
    public class HomeController(IHttpClientFactory httpClientFactory, 
        ILogger<GatewayController> logger) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}