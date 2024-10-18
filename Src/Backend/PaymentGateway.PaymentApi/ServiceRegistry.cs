using PaymentGateway.Application.Security.Users.Queries;
using PaymentGateway.Domain;
using PaymentGateway.Infrastructure;
using PaymentGateway.PaymentApi.PaymentType.Concrete;
using PaymentGateway.PaymentApi.PaymentType.Factory;

namespace PaymentGateway.PaymentApi
{
    public static class ServiceRegistry
    {
        public static IServiceCollection AddServiceRegistry(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient();


            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            var applicationAssembly = typeof(LoginQuery).Assembly;
            builder.Services.AddAutoMapper(applicationAssembly);
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

            return builder.Services;
        }

        public static IServiceCollection AddFactoryServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<PaymentFactory>();
            builder.Services.AddScoped<ConcreteAqayepardakht>();

            return builder.Services;
        }
    }
}
