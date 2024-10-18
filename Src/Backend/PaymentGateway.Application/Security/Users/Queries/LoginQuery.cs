using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Security.Users;
using PaymentGateway.Domain.Security.Users.Dto;

namespace PaymentGateway.Application.Security.Users.Queries
{
    public class LoginQuery : LoginRequestDto, IRequest<User?>
    {
    }

    public class LoginQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<LoginQuery, User?>
    {
        public async Task<User?> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.UserRepository.Login(request.UserName, request.Password);
        }
    }
}
