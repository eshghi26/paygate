using Dapper;
using PaymentGateway.Domain.Payment.TokenInfos;
using PaymentGateway.Infrastructure.Helper;

namespace PaymentGateway.Infrastructure.Payment.TokenInfos
{
    public class TokenInfoRepository : ITokenInfoRepository
    {
        #region DataMember
        private const string TableName = @"""Payment"".""TokenInfos""";
        #endregion

        #region Fetch
        public async Task<TokenInfo?> GetById(long id)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<TokenInfo>($@"Select * From {TableName} WHERE ""Id"" = @id And ""IsDeleted"" = false", new { id });

            return result.SingleOrDefault();
        }

        public async Task<TokenInfo?> GetByToken(string token)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var uuId = Guid.Parse(token);
            var result = await db.QueryAsync<TokenInfo>($@"Select * From {TableName} WHERE ""Token"" = @Token",
                new
                {
                    Token = uuId
                });

            return result.SingleOrDefault();
        }
        #endregion

        #region Insert
        public async Task<int> Insert(TokenInfo entity)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var prams = new DynamicParameters();
            prams.Add("@Token", entity.Token);
            prams.Add("@Amount", entity.Amount);
            prams.Add("@CallBackUrl", entity.CallBackUrl);
            prams.Add("@TrCode", entity.TrCode);
            prams.Add("@ReserveNumber", entity.ReserveNumber);
            prams.Add("@ExpireDate", entity.ExpireDate);
            prams.Add("@MerchantId", entity.MerchantId);
            prams.Add("@GatewayId", entity.GatewayId);
            prams.Add("@CreateOn", DateTime.Now);
            prams.Add("@MerchantUserId", entity.MerchantUserId);
            prams.Add("@Captcha", entity.Captcha);
            prams.Add("@Obj", entity.Obj);
            prams.Add("@ExtraValue1", entity.ExtraValue1);
            prams.Add("@ExtraValue2", entity.ExtraValue2);
            prams.Add("@InvoiceNumber", entity.InvoiceNumber);

            var entityId = (await db.QueryAsync<int>(
                $@"INSERT INTO {TableName} 
                               (
                                       ""Token""
                                      ,""Amount""
                                      ,""CallBackUrl""
                                      ,""TrCode""
                                      ,""ReserveNumber""
                                      ,""ExpireDate""
                                      ,""MerchantId""
                                      ,""GatewayId""
                                      ,""CreateOn""
                                      ,""MerchantUserId""
                                      ,""Captcha""
                                      ,""Obj""
                                      ,""ExtraValue1""
                                      ,""ExtraValue2""
                                      ,""InvoiceNumber""
                               )
                               VALUES
                               (
                                       @Token
                                      ,@Amount
                                      ,@CallBackUrl
                                      ,@TrCode
                                      ,@ReserveNumber
                                      ,@ExpireDate
                                      ,@MerchantId
                                      ,@GatewayId
                                      ,@CreateOn
                                      ,@MerchantUserId
                                      ,@Captcha
                                      ,@Obj
                                      ,@ExtraValue1
                                      ,@ExtraValue2
                                      ,@InvoiceNumber
                               );
                               SELECT currval('""Payment"".""TokenInfos_Id_seq""');", prams)).SingleOrDefault();

            return entityId;
        }
        #endregion

        #region Update
        public async Task<bool> Update(TokenInfo entity)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        ""Token"" = @Token
                                       ,""Amount"" = @Amount
                                       ,""CallBackUrl"" = @CallBackUrl
                                       ,""TrCode"" = @TrCode
                                       ,""ReserveNumber"" = @ReserveNumber
                                       ,""ExpireDate"" = @ExpireDate
                                       ,""MerchantId"" = @MerchantId
                                       ,""GatewayId"" = @GatewayId
                                       ,""MerchantUserId"" = @MerchantUserId
                                       ,""Captcha"" = @Captcha
                                       ,""Obj"" = @Obj
                                       ,""ExtraValue1"" = @ExtraValue1
                                       ,""ExtraValue2"" = @ExtraValue2
                                       ,""InvoiceNumber"" = @InvoiceNumber
                                   WHERE ""Id"" = @Id";

            var rowsAffected = await db.ExecuteAsync(sqlQuery, new
            {
                entity.Token,
                entity.Amount,
                entity.CallBackUrl,
                entity.TrCode,
                entity.ReserveNumber,
                entity.ExpireDate,
                entity.MerchantId,
                entity.GatewayId,
                entity.MerchantUserId,
                entity.Captcha,
                entity.Obj,
                entity.ExtraValue1,
                entity.ExtraValue2,
                entity.InvoiceNumber,
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

        public async Task<bool> SoftDelete(long id)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var sqlQuery = $@"UPDATE {TableName} 
                                SET ""IsDeleted"" = true                                       
                                WHERE ""Id"" = @Id";

            var rowsAffected = await db.ExecuteAsync(sqlQuery, new
            {
                Id = id
            });

            return rowsAffected > 0;
        }
        #endregion
    }
}