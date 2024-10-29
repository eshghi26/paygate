using Common.Helper.Enum;
using Common.Helper.Extension;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace PaymentGateway.PaymentApi.Controllers
{
    public class BaseController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult ExceptionError(string exceptionMessage, ErrorType type = ErrorType.Toast)
        {
            return CustomError(exceptionMessage, HttpStatusCode.ExpectationFailed, type);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult FailedOperation(string message, ErrorType type = ErrorType.Toast)
        {
            return StatusCode((int)HttpStatusCode.ExpectationFailed, message);

            //return CustomError(message, HttpStatusCode.ExpectationFailed, type);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult ValidationError(KeyValuePair<string, string>[] errors, ErrorType type = ErrorType.Toast)
        {
            var reasonObject = System.Text.Json.JsonSerializer.Deserialize<object?>(errors.ParseValidationError());
            return CustomError(reasonObject, HttpStatusCode.UnprocessableEntity, type);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult ValidationError(string[,] err)
        {
            var lst = new List<string>();
            for (var i = 0; i < err.GetLength(0); i++)
            {
                lst.Add($"""
                 "{err[i, 0]}":"{err[i, 1]}"
                 """);
            }

            var jsStr = string.Empty;
            if (lst.Count != 0)
            {
                jsStr = $@"{{""errors"": {{{string.Join(',', lst)}}}}}";

            }
            var reasonObject = System.Text.Json.JsonSerializer.Deserialize<object?>(jsStr);
            return CustomError(reasonObject, HttpStatusCode.UnprocessableEntity, ErrorType.SnakeBar);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult ValidationError(string jsonString, ErrorType type = ErrorType.Toast)
        {
            var jsonObject = System.Text.Json.JsonSerializer.Deserialize<object?>($"{{{jsonString}}}");

            return CustomError(jsonObject, HttpStatusCode.UnprocessableEntity, type);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult UnauthorizedError(ErrorType type = ErrorType.Toast)
        {
            return CustomError("Unauthorized", HttpStatusCode.Unauthorized, type);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult AccessDeniedError(ErrorType type = ErrorType.Toast)
        {
            return CustomError("Access denied", HttpStatusCode.Forbidden, type);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult NotFoundContent(ErrorType type = ErrorType.Toast)
        {
            return CustomError("Not found content", HttpStatusCode.NotFound, type);
        }

        private ObjectResult CustomError(string message, HttpStatusCode status = HttpStatusCode.BadRequest, ErrorType type = ErrorType.Toast)
        {
            return StatusCode((int)status, new
            {
                Message = message,
                Type = type,
                Internal = true
            });
        }

        private ObjectResult CustomError(object? obj, HttpStatusCode status = HttpStatusCode.BadRequest, ErrorType type = ErrorType.Toast)
        {
            return StatusCode((int)status, new
            {
                Message = obj,
                Type = type,
                Internal = true
            });
        }
    }
}
