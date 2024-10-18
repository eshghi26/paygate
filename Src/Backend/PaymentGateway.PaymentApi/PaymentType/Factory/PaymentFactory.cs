using Common.Helper.Enum;
using PaymentGateway.PaymentApi.PaymentType.Concrete;

namespace PaymentGateway.PaymentApi.PaymentType.Factory
{
    public class PaymentFactory (IServiceProvider serviceProvider)
    {
        public IPayment CreatePaymentFactory(short gatewayType, short gatewayModuleType)
        {
            if ((GatewayType)gatewayType == GatewayType.PspSkin)
            {
                switch ((GatewayModuleType)gatewayModuleType)
                {
                    case GatewayModuleType.Aqayepardakht:
                        return serviceProvider.GetRequiredService<ConcreteAqayepardakht>();
                }
            }

            throw new ArgumentException("Invalid payment type");
        }
    }
}
