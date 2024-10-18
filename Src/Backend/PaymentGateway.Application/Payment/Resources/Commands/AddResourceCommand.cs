using AutoMapper;
using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.Resources;

namespace PaymentGateway.Application.Payment.Resources.Commands
{
    public class AddResourceCommand : Resource, IRequest<long>
    {
    }

    public class AddResourceCommandHandler (IUnitOfWork unitOfWork, IMapper mapper) 
        : IRequestHandler<AddResourceCommand, long>
    {
        public async Task<long> Handle(AddResourceCommand request, CancellationToken cancellationToken)
        {
            var entity = mapper.Map<Resource>(request);
            return await unitOfWork.ResourceRepository.Insert(entity);
        }
    }
}
