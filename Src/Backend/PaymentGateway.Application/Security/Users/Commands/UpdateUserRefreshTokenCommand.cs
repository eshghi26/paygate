using MediatR;
using PaymentGateway.Domain;

namespace PaymentGateway.Application.Security.Users.Commands
{
    public class UpdateUserRefreshTokenCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string? RefreshTokenSerial { get; set; }
        public DateTime? RefreshTokenExpiryDate { get; set; }
    }

    public class UpdateUserRefreshTokenCommandHandler (IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserRefreshTokenCommand, bool>
    {
        public async Task<bool> Handle(UpdateUserRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await unitOfWork.UserRepository
                .UpdateRefreshToken(request.Id, request.RefreshTokenSerial, request.RefreshTokenExpiryDate);
        }
    }
}
