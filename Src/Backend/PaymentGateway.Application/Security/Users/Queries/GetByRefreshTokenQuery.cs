using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Security.Users;

namespace PaymentGateway.Application.Security.Users.Queries
{
    public class GetByRefreshTokenQuery : IRequest<User?>
    {
        public required string RefreshToken { get; set; }
    }

    public class GetByRefreshTokenQueryHandler (IUnitOfWork unitOfWork)
    : IRequestHandler<GetByRefreshTokenQuery, User?>
    {
        public async Task<User?> Handle(GetByRefreshTokenQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.UserRepository.GetByRefreshTokenSerial(request.RefreshToken);
        }
    }
}
