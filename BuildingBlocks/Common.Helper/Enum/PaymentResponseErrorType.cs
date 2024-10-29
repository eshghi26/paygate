namespace Common.Helper.Enum
{
    public enum PaymentResponseErrorType
    {
        Unknown = 0,
        RequiredKey = 1,
        RequiredAmount = 2,
        RequiredCallback = 3,
        OutOffRangeAmount = 4,
        DeactivateMerchant = 5,
        NotFindGateway = 6,
        DeactivateGateway = 7,
        RequiredToken = 8,
        NotFindTransaction = 9,
        TransactionHasAlreadyBeenVerified = 10,
        TransactionStatusIsNotVerifiable = 11,
        InvalidAmount = 12,
    }
}