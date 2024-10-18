using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.Payment.Resources.Queries;

namespace PaymentGateway.PaymentApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ResourceController (IMediator mediator): BaseController
    {
        [HttpGet]
        [Route("GetResource/{name}")]
        public async Task<IActionResult> GetResource(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return NotFound();
                }

                if (name.Contains('.'))
                {
                    var namePart = name.Split('.');

                    if (namePart.Length > 0)
                    {
                        name = namePart[0];
                    }
                }

                var resource = await mediator.Send(new GetResourceByNameQuery
                {
                    Name = name
                });

                if (resource == null)
                {
                    return NotFound();
                }

                var contentType = "image/jpeg";
                return File(resource.FileData, contentType);
            }
            catch (Exception exp)
            {
                return NotFound();
            }
        }
    }
}
