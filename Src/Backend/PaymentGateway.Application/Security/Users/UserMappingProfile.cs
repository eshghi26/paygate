using AutoMapper;
using PaymentGateway.Application.Security.Users.Queries;
using PaymentGateway.Domain.Security.Roles;
using PaymentGateway.Domain.Security.Users.Dto;

namespace PaymentGateway.Application.Security.Users
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<LoginQuery, LoginRequestDto>().ReverseMap();
        }
    }
}