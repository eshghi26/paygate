namespace Common.Helper.Enum
{
    public enum PaymentResponseErrorType
    {
        Unknown = 0,
        RequiredKey = -1,
        RequiredAmount = -2,
        RequiredCallback = -3,
        OutOffRangeAmount = -4,
        DeactivateMerchant = -5,
        NotFindGateway = -6,
        DeactivateGateway = -7,
        
    }
}