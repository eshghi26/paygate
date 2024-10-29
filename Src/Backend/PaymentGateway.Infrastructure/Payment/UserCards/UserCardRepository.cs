using Dapper;
using PaymentGateway.Domain.Payment.UserCards;
using PaymentGateway.Infrastructure.Helper;

namespace PaymentGateway.Infrastructure.Payment.UserCards
{
    public class UserCardRepository : IUserCardRepository
    {
        #region DataMember
        private const string TableName = @"""Payment"".""UserCards""";
        #endregion

        #region Fetch
        public async Task<UserCard?> GetById(long id)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<UserCard>($@"Select * From {TableName} WHERE ""Id"" = @id And ""IsDeleted"" = false", new { id });

            return result.SingleOrDefault();
        }

        public async Task<List<UserCard>?> GetByUserId(string userId, int merchantId)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<UserCard>($@"Select * From {TableName} WHERE ""UserId"" = @userId And ""MerchantId"" = @merchantId And ""IsDeleted"" = false", new { userId, merchantId });

            return result.ToList();
        }
        #endregion

        #region Insert
        public async Task<int> Insert(UserCard entity)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var prams = new DynamicParameters();
            prams.Add("@MerchantId", entity.MerchantId);
            prams.Add("@UserId", entity.UserId);
            prams.Add("@Pan", entity.Pan);
            prams.Add("@Cvv2", entity.Cvv2);
            prams.Add("@ExpYear", entity.ExpYear);
            prams.Add("@ExpMonth", entity.ExpMonth);
            prams.Add("@Displayable", entity.Displayable);
            prams.Add("@CreateOn", DateTime.Now);
            prams.Add("@IsDeleted", false);

            var entityId = (await db.QueryAsync<int>(
                $@"INSERT INTO {TableName} 
                               (
                                       ""MerchantId""
                                      ,""UserId""
                                      ,""Pan""
                                      ,""Cvv2""
                                      ,""ExpYear""
                                      ,""ExpMonth""
                                      ,""Displayable""
                                      ,""CreateOn""
                                      ,""IsDeleted""
                               )
                               VALUES
                               (
                                       @MerchantId
                                      ,@UserId
                                      ,@Pan
                                      ,@Cvv2
                                      ,@ExpYear
                                      ,@ExpMonth
                                      ,@Displayable
                                      ,@CreateOn
                                      ,@IsDeleted
                               );
                               SELECT currval('""Payment"".""UserCards_Id_seq""');", prams)).SingleOrDefault();

            return entityId;
        }
        #endregion

        #region Update
        public async Task<bool> Update(UserCard entity)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        ""MerchantId"" = @MerchantId
                                       ,""UserId"" = @UserId
                                       ,""Pan"" = @Pan
                                       ,""Cvv2"" = @Cvv2
                                       ,""ExpYear"" = @ExpYear
                                       ,""ExpMonth"" = @ExpMonth
                                       ,""Displayable"" = @Displayable
                                   WHERE ""Id"" = @Id";

            var rowsAffected = await db.ExecuteAsync(sqlQuery, new
            {
                entity.MerchantId,
                entity.UserId,
                entity.Pan,
                entity.Cvv2,
                entity.ExpYear,
                entity.ExpMonth,
                entity.Displayable,
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