using Dapper;
using PaymentGateway.Domain.Payment.Transactions;
using PaymentGateway.Infrastructure.Helper;

namespace PaymentGateway.Infrastructure.Payment.Transactions
{
    public class TransactionRepository : ITransactionRepository
    {
        #region DataMember
        private const string TableName = @"""Payment"".""Transactions""";
        #endregion

        #region Fetch
        public async Task<Transaction?> GetById(long id)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<Transaction>($@"Select * From {TableName} WHERE ""Id"" = @id", new { id });

            return result.SingleOrDefault();
        }

        public async Task<Transaction?> GetByToken(Guid token)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<Transaction>($@"Select * From {TableName} WHERE ""Token"" = @token", new { token });

            return result.FirstOrDefault();
        }

        public async Task<Transaction?> GetByTransactionCode(string transactionCode)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<Transaction>($@"Select * From {TableName} WHERE ""TransactionCode"" = @transactionCode", new { transactionCode });

            return result.FirstOrDefault();
        }
        #endregion

        #region Insert
        public async Task<long> Insert(Transaction entity)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var prams = new DynamicParameters();
            prams.Add("@ParentId", entity.ParentId);
            prams.Add("@Status", entity.Status);
            prams.Add("@OperationType", entity.OperationType);
            prams.Add("@Amount", entity.Amount);
            prams.Add("@MerchantId", entity.MerchantId);
            prams.Add("@GatewayId", entity.GatewayId);
            prams.Add("@Token", entity.Token);
            prams.Add("@TransactionCode", entity.TransactionCode);
            prams.Add("@ReserveNumber", entity.ReserveNumber);
            prams.Add("@InvoiceNumber", entity.InvoiceNumber);
            prams.Add("@WageAmount", entity.WageAmount);
            prams.Add("@GetOtpDate", entity.GetOtpDate);
            prams.Add("@FinishTransactionDate", entity.FinishTransactionDate);
            prams.Add("@CancelTransactionDate", entity.CancelTransactionDate);
            prams.Add("@VerifyDate", entity.VerifyDate);
            prams.Add("@VerifyRetryCount", entity.VerifyRetryCount);
            prams.Add("@FinalAmount", entity.FinalAmount);
            prams.Add("@Description", entity.Description);
            prams.Add("@CreateOn", DateTime.Now);
            prams.Add("@MerchantUserId", entity.MerchantUserId);
            prams.Add("@TrackingNumber", entity.TrackingNumber);

            var entityId = (await db.QueryAsync<int>(
                $@"INSERT INTO {TableName} 
                               (
                                       ""ParentId""
                                      ,""Status""
                                      ,""OperationType""
                                      ,""Amount""
                                      ,""MerchantId""
                                      ,""GatewayId""
                                      ,""Token""
                                      ,""TransactionCode""
                                      ,""ReserveNumber""
                                      ,""InvoiceNumber""
                                      ,""WageAmount""
                                      ,""GetOtpDate""
                                      ,""FinishTransactionDate""
                                      ,""CancelTransactionDate""
                                      ,""VerifyDate""
                                      ,""VerifyRetryCount""
                                      ,""FinalAmount""
                                      ,""Description""
                                      ,""CreateOn""
                                      ,""MerchantUserId""
                                      ,""TrackingNumber""
                               )
                               VALUES
                               (
                                       @ParentId
                                      ,@Status
                                      ,@OperationType
                                      ,@Amount
                                      ,@MerchantId
                                      ,@GatewayId
                                      ,@Token
                                      ,@TransactionCode
                                      ,@ReserveNumber
                                      ,@InvoiceNumber
                                      ,@WageAmount
                                      ,@GetOtpDate
                                      ,@FinishTransactionDate
                                      ,@CancelTransactionDate
                                      ,@VerifyDate
                                      ,@VerifyRetryCount
                                      ,@FinalAmount
                                      ,@Description
                                      ,@CreateOn
                                      ,@MerchantUserId
                                      ,@TrackingNumber
                               );
                               SELECT currval('""Payment"".""Transaction_Id_seq""');", prams)).SingleOrDefault();

            return entityId;
        }
        #endregion

        #region Update
        public async Task<bool> Update(Transaction entity)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        ""ParentId"" = @ParentId
                                       ,""Status"" = @Status
                                       ,""OperationType"" = @OperationType
                                       ,""Amount"" = @Amount
                                       ,""MerchantId"" = @MerchantId
                                       ,""GatewayId"" = @GatewayId
                                       ,""Token"" = @Token
                                       ,""TransactionCode"" = @TransactionCode
                                       ,""ReserveNumber"" = @ReserveNumber
                                       ,""InvoiceNumber"" = @InvoiceNumber
                                       ,""WageAmount"" = @WageAmount
                                       ,""GetOtpDate"" = @GetOtpDate
                                       ,""FinishTransactionDate"" = @FinishTransactionDate
                                       ,""CancelTransactionDate"" = @CancelTransactionDate
                                       ,""VerifyDate"" = @VerifyDate
                                       ,""VerifyRetryCount"" = @VerifyRetryCount
                                       ,""FinalAmount"" = @FinalAmount
                                       ,""Description"" = @Description
                                       ,""MerchantUserId"" = @MerchantUserId
                                       ,""TrackingNumber"" = @TrackingNumber
                                   WHERE ""Id"" = @Id";

            var rowsAffected = await db.ExecuteAsync(sqlQuery, new
            {
                entity.ParentId,
                entity.Status,
                entity.OperationType,
                entity.Amount,
                entity.MerchantId,
                entity.GatewayId,
                entity.Token,
                entity.TransactionCode,
                entity.ReserveNumber,
                entity.InvoiceNumber,
                entity.WageAmount,
                entity.GetOtpDate,
                entity.FinishTransactionDate,
                entity.CancelTransactionDate,
                entity.VerifyDate,
                entity.VerifyRetryCount,
                entity.FinalAmount,
                entity.Description,
                entity.MerchantUserId,
                entity.TrackingNumber,
                entity.Id
            });

            return rowsAffected > 0;
        }
        #endregion

        #region Delete
        public async Task<bool> Delete(long id)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var sqlQuery = $@"DELETE FROM {TableName} WHERE ""Id"" = @Id";
            var rowsCount = await db.ExecuteAsync(sqlQuery, new { id });
            return rowsCount > 0;
        }
        #endregion
    }
}