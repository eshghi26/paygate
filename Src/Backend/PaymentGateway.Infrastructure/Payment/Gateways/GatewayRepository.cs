using Dapper;
using PaymentGateway.Infrastructure.Helper;
using PaymentGateway.Domain.Payment.Gateways;

namespace PaymentGateway.Infrastructure.Payment.Gateways
{
    public class GatewayRepository : IGatewayRepository
    {
        #region DataMember
        private const string TableName = @"""Payment"".""Gateways""";
        #endregion

        #region Fetch
        public async Task<Gateway?> GetById(long id)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<Gateway>($@"Select * From {TableName} WHERE ""Id"" = @id And ""IsDeleted"" = false", new { id });

            return result.SingleOrDefault();
        }

        public async Task<List<Gateway>> GetListByMerchantId(int merchantId)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<Gateway>($@"Select * From {TableName} WHERE ""MerchantId"" = @merchantId And ""IsDeleted"" = false", new { merchantId });

            return result.ToList();
        }

        public async Task<Gateway?> GetByKey(string key)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<Gateway>($@"Select * From {TableName} WHERE ""Key"" = @key And ""IsDeleted"" = false", new { key });

            return result.SingleOrDefault();
        }
        #endregion

        #region Insert
        public async Task<int> Insert(Gateway entity)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var prams = new DynamicParameters();
            prams.Add("@Name", entity.Name);
            prams.Add("@Key", entity.Key);
            prams.Add("@MerchantId", entity.MerchantId);
            prams.Add("@Type", entity.Type);
            prams.Add("@MinAmount", entity.MinAmount);
            prams.Add("@MaxAmount", entity.MaxAmount);
            prams.Add("@Status", entity.Status);
            prams.Add("@ModuleType", entity.ModuleType);
            prams.Add("@Pin", entity.Pin);
            prams.Add("@UserName", entity.UserName);
            prams.Add("@Password", entity.Password);
            prams.Add("@TerminalNumber", entity.TerminalNumber);
            prams.Add("@CallbackUrl", entity.CallbackUrl);
            prams.Add("@BankCallbackUrl", entity.BankCallbackUrl);
            prams.Add("@Comment", entity.Comment);
            prams.Add("@CreateOn", DateTime.Now);
            prams.Add("@IsDeleted", false);

            var entityId = (await db.QueryAsync<int>(
                $@"INSERT INTO {TableName} 
                               (
                                       ""Name""
                                      ,""Key""
                                      ,""MerchantId""
                                      ,""Type""
                                      ,""MinAmount""
                                      ,""MaxAmount""
                                      ,""Status""
                                      ,""ModuleType""
                                      ,""Pin""
                                      ,""UserName""
                                      ,""Password""
                                      ,""TerminalNumber""
                                      ,""CallbackUrl""
                                      ,""BankCallbackUrl""
                                      ,""Comment""
                                      ,""CreateOn""
                                      ,""IsDeleted""
                               )
                               VALUES
                               (
                                       @Name
                                      ,@Key
                                      ,@MerchantId
                                      ,@Type
                                      ,@MinAmount
                                      ,@MaxAmount
                                      ,@Status
                                      ,@ModuleType
                                      ,@Pin
                                      ,@UserName
                                      ,@Password
                                      ,@TerminalNumber
                                      ,@CallbackUrl
                                      ,@BankCallbackUrl
                                      ,@Comment
                                      ,@CreateOn
                                      ,@IsDeleted
                               );
                               SELECT currval('""Payment"".""Gateways_Id_seq""');", prams)).SingleOrDefault();

            return entityId;
        }
        #endregion

        #region Update
        public async Task<bool> Update(Gateway entity)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        ""Name"" = @Name
                                       ,""Key"" = @Key
                                       ,""MerchantId"" = @MerchantId
                                       ,""Type"" = @Type
                                       ,""MinAmount"" = @MinAmount
                                       ,""MaxAmount"" = @MaxAmount
                                       ,""Status"" = @Status
                                       ,""ModuleType"" = @ModuleType
                                       ,""Pin"" = @Pin
                                       ,""UserName"" = @UserName
                                       ,""Password"" = @Password
                                       ,""TerminalNumber"" = @TerminalNumber
                                       ,""CallbackUrl"" = @CallbackUrl
                                       ,""BankCallbackUrl"" = @BankCallbackUrl
                                       ,""Comment"" = @Comment
                                   WHERE ""Id"" = @Id";

            var rowsAffected = await db.ExecuteAsync(sqlQuery, new
            {
                entity.Name,
                entity.Key,
                entity.MerchantId,
                entity.Type,
                entity.MinAmount,
                entity.MaxAmount,
                entity.Status,
                entity.ModuleType,
                entity.Pin,
                entity.UserName,
                entity.Password,
                entity.TerminalNumber,
                entity.CallbackUrl,
                entity.BankCallbackUrl,
                entity.Comment,
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