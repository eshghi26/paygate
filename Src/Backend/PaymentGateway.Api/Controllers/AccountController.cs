using AutoMapper;
using Common.Helper.Helper;
using Logging.NLog;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Helper;
using PaymentGateway.Application.Security.Users.Commands;
using PaymentGateway.Application.Security.Users.Queries;
using PaymentGateway.Domain.Security.Users.Dto;

namespace PaymentGateway.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController 
        (IMediator mediator, IMapper mapper, ITokenService tokenService, ILoggerManager logger): BaseController
    {
        #region Login
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
        {
            try
            {
                #region Validation
                var entityValidation = EntityValidation(request);
                if (entityValidation != null && entityValidation.Any())
                {
                    return ValidationError(entityValidation);
                }

                //var recaptchaSecretKey = config["ApplicationSettings:RecaptchaSecretKey"];
                //if (!string.IsNullOrEmpty(recaptchaSecretKey?.Trim()))
                //{
                    //   var isHuman = await CaptchaHelper.IsValidCaptcha(request.CaptchaCode, recaptchaSecretKey.Trim());

                    //    if (!isHuman)
                    //    {
                    //        return ValidationError(new[,] { { "captcha", "Captcha is required" } });
                    //    }
                //}
                #endregion

                request.Password = SecurityHelper.AdvancePasswordHash(request.Password);

                var query = mapper.Map<LoginQuery>(request);
                var user = await mediator.Send(query);

                if (user == null)
                {
                    return FailedOperation("User name or password is incorrect");
                }

                if (!user.IsActive)
                {
                    return FailedOperation("Currently, this user is deactivated");
                }

                if (user.IsBan)
                {
                    return FailedOperation("Currently, this user is banned");
                }

                var token = tokenService.CreateToken(user.Id, user.Username);

                if (token == null)
                {
                    return FailedOperation("Currently unable to log in due to a technical problem");
                }

                var updateUserCommand = new UpdateUserRefreshTokenCommand
                {
                    Id = user.Id,
                    RefreshTokenSerial = token.RefreshTokenSerial,
                    RefreshTokenExpiryDate = token.RefreshTokenExpiryDate
                };

                var updated = await mediator.Send(updateUserCommand);

                if (!updated)
                {
                    return FailedOperation("Currently unable to log in due to a technical problem in data base");
                }

                return new LoginResponseDto
                {
                    UserId = user.Id,
                    Token = token.AccessToken,
                    RefreshToken = token.RefreshTokenSerial
                };
            }
            catch (Exception exp)
            {
                logger.LogError($"{exp.Message}{Environment.NewLine}{exp.StackTrace}");
                return ExceptionError("Exception in operator");
            }
        }

        private KeyValuePair<string, string>[]? EntityValidation(LoginRequestDto entity)
        {
            var validations = new List<KeyValuePair<string, string>>();

            if (string.IsNullOrEmpty(entity.UserName.Trim()))
            {
                validations.Add(new KeyValuePair<string, string>("userName", "UserName is required"));
            }

            if (string.IsNullOrEmpty(entity.Password))
            {
                validations.Add(new KeyValuePair<string, string>("password", "Password is required"));
            }
            else
            {
                if (entity.Password.Length < 6)
                {
                    validations.Add(new KeyValuePair<string, string>("password", "Password must have more than 6 characters"));
                }
            }

            if (string.IsNullOrEmpty(entity.CaptchaCode?.Trim()))
            {
                validations.Add(new KeyValuePair<string, string>("captcha", "Captcha is required"));
            }

            return validations.Any() ? validations.ToArray() : null;
        }
        #endregion

        #region Refresh Token
        [HttpGet]
        [Route("RefreshToken/{refreshToken}")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> RefreshToken(string refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return ValidationError(new[,] { { "refreshToken", "RefreshToken is required" } });
                }

                var user = await mediator.Send(new GetByRefreshTokenQuery { RefreshToken = refreshToken});

                if (user == null ||
                    !user.IsActive ||
                    user.IsBan ||
                    !user.RefreshTokenExpiryDate.HasValue ||
                    user.RefreshTokenExpiryDate.Value < DateTime.UtcNow)
                {
                    return UnauthorizedError();
                }

                var token = tokenService.CreateToken(user.Id, user.Username);

                if (token == null)
                {
                    return FailedOperation("Can not create token");
                }

                var updateEmployeeCommand = new UpdateUserRefreshTokenCommand
                {
                    Id = user.Id,
                    RefreshTokenSerial = token.RefreshTokenSerial,
                    RefreshTokenExpiryDate = token.RefreshTokenExpiryDate
                };

                var updated = await mediator.Send(updateEmployeeCommand);

                if (!updated)
                {
                    return FailedOperation("Can not update token on database");
                }

                return new LoginResponseDto
                {
                    UserId = user.Id,
                    Token = token.AccessToken,
                    RefreshToken = token.RefreshTokenSerial
                };
            }
            catch (Exception exp)
            {
                logger.LogError($"{exp.Message}{Environment.NewLine}{exp.StackTrace}");
                return ExceptionError("Exception in operator");
            }
        }
        #endregion

        #region LogOut
        [HttpGet]
        [Route("Logout")]
        public async Task<ActionResult<bool>> Logout()
        {
            try
            {
                #region Validation
                if (UserId <= 0)
                {
                    return UnauthorizedError();
                }
                #endregion

                var user = await mediator.Send(new GetUserByIdQuery { Id = UserId });
                if (user == null)
                {
                    return false;
                }

                var updateEmployeeCommand = new UpdateUserRefreshTokenCommand
                {
                    Id = UserId,
                    RefreshTokenSerial = null,
                    RefreshTokenExpiryDate = null
                };

                var updated = await mediator.Send(updateEmployeeCommand);

                return updated;
            }
            catch (Exception exp)
            {
                logger.LogError($"{exp.Message}{Environment.NewLine}{exp.StackTrace}");
                return ExceptionError("Exception in operator");
            }
        }
        #endregion
    }
}