using Dapper;
using PaymentGateway.Domain.Payment.Merchants;
using PaymentGateway.Infrastructure.Helper;

namespace PaymentGateway.Infrastructure.Payment.Merchants
{
    public class MerchantRepository : IMerchantRepository
    {
        #region DataMember
        private const string TableName = @"""Payment"".""Merchants""";
        #endregion

        #region Fetch
        public async Task<Merchant?> GetById(long id)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<Merchant>($@"Select * From {TableName} WHERE ""Id"" = @id And ""IsDeleted"" = false", new { id });

            return result.SingleOrDefault();
        }
        #endregion

        #region Insert
        public async Task<int> Insert(Merchant entity)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var prams = new DynamicParameters();
            prams.Add("@Name", entity.Name);
            prams.Add("@WageLimit", entity.WageLimit);
            prams.Add("@Logo", entity.Logo);
            prams.Add("@ThumbnailLogo", entity.ThumbnailLogo);
            prams.Add("@Comment", entity.Comment);
            prams.Add("@IsActive", entity.IsActive);
            prams.Add("@CreateOn", DateTime.Now);
            prams.Add("@IsDeleted", false);

            var entityId = (await db.QueryAsync<int>(
                $@"INSERT INTO {TableName} 
                               (
                                       ""Name""
                                      ,""WageLimit""
                                      ,""Logo""
                                      ,""ThumbnailLogo""
                                      ,""Comment""
                                      ,""IsActive""
                                      ,""CreateOn""
                                      ,""IsDeleted""
                               )
                               VALUES
                               (
                                       @Name
                                      ,@WageLimit
                                      ,@Logo
                                      ,@ThumbnailLogo
                                      ,@Comment
                                      ,@IsActive
                                      ,@CreateOn
                                      ,@IsDeleted
                               );
                               SELECT currval('""Payment"".""Merchants_Id_seq""');", prams)).SingleOrDefault();

            return entityId;
        }
        #endregion

        #region Update
        public async Task<bool> Update(Merchant entity)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        ""Name"" = @Name
                                       ,""WageLimit"" = @WageLimit
                                       ,""Logo"" = @Logo
                                       ,""ThumbnailLogo"" = @ThumbnailLogo
                                       ,""Comment"" = @Comment
                                       ,""IsActive"" = @IsActive
                                   WHERE ""Id"" = @Id";

            var rowsAffected = await db.ExecuteAsync(sqlQuery, new
            {
                entity.Name,
                entity.WageLimit,
                entity.Logo,
                entity.ThumbnailLogo,
                entity.Comment,
                entity.IsActive,
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