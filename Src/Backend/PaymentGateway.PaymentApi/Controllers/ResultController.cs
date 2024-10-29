using Microsoft.AspNetCore.Mvc;
using PaymentGateway.PaymentApi.PaymentType.Concrete.Aqayepardakht.Dto;

namespace PaymentGateway.PaymentApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ResultController : ControllerBase
    {
        #region Aqayepardakht
        [HttpPost]
        [Route("Aqayepardakht")]
        public async Task Aqayepardakht([FromForm] AqCallbackRequest request)
        {
            try
            {

            }
            catch (Exception exp)
            {
                
            }
        }
        #endregion
    }
}
