using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PaymentGateway.PaymentApi.Helper
{
    public class SecretKeyCheckAttribute() 
        : TypeFilterAttribute(typeof(SecretKeyCheckFilter))
    {
        public class SecretKeyCheckFilter : IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext context)
            {
                try
                {
                    var secretKey = context.HttpContext.Request.Headers["SecretKey"];

                    if (string.IsNullOrEmpty(secretKey))
                    {
                        context.Result = new UnauthorizedResult();
                        return;
                    }

                    if (secretKey != "N17NSpgNUn6N0OZ4jGoXDHsb4MDRiftD!eeDe")
                        context.Result = new UnauthorizedResult();
                }
                catch (Exception ex)
                {
                    context.Result = new UnauthorizedResult();
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }
        }
    }
}
