using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Security.Users;

namespace PaymentGateway.Application.Security.Users.Queries
{
    public class GetUserByIdQuery : IRequest<User?>
    {
        public long Id { get; set; }
    }

    public class GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetUserByIdQuery, User?>
    {
        public async Task<User?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.UserRepository.GetById(request.Id);
        }
    }
}
