using AutoMapper;
using PaymentGateway.Application.Payment.TokenInfos.Commands;
using PaymentGateway.Domain.Payment.TokenInfos;

namespace PaymentGateway.Application.Payment.TokenInfos
{
    public class TokenInfoMappingProfile : Profile
    {
        public TokenInfoMappingProfile()
        {
            CreateMap<EditTokenCommand, TokenInfo>().ReverseMap();
        }
    }
}
