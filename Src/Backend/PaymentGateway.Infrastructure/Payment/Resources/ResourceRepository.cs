using Dapper;
using PaymentGateway.Domain.Payment.Resources;
using PaymentGateway.Infrastructure.Helper;

namespace PaymentGateway.Infrastructure.Payment.Resources
{
    public class ResourceRepository : IResourceRepository
    {
        #region DataMember
        private const string TableName = @"""Payment"".""Resources""";
        #endregion

        #region Fetch
        public async Task<Resource?> GetById(long id)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<Resource>($@"Select * From {TableName} WHERE ""Id"" = @id", new { id });

            return result.SingleOrDefault();
        }

        public async Task<Resource?> GetByName(string name)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<Resource>($@"Select * From {TableName} WHERE ""Name"" = @name", new { name });

            return result.FirstOrDefault();
        }
        #endregion

        #region Insert
        public async Task<int> Insert(Resource entity)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var prams = new DynamicParameters();
            prams.Add("@Name", entity.Name);
            prams.Add("@FileData", entity.FileData);
            prams.Add("@CreateOn", DateTime.Now);
            prams.Add("@ExpireDate", entity.ExpireDate);

            var entityId = (await db.QueryAsync<int>(
                $@"INSERT INTO {TableName} 
                               (
                                       ""Name""
                                      ,""FileData""
                                      ,""CreateOn""
                                      ,""ExpireDate""
                               )
                               VALUES
                               (
                                       @Name
                                      ,@FileData
                                      ,@CreateOn
                                      ,@ExpireDate
                               );
                               SELECT currval('""Payment"".""Resources_Id_seq""');", prams)).SingleOrDefault();

            return entityId;
        }
        #endregion

        #region Update
        public async Task<bool> Update(Resource entity)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        ""Name"" = @Name
                                       ,""FileData"" = @FileData
                                       ,""ExpireDate"" = @ExpireDate
                                   WHERE ""Id"" = @Id";

            var rowsAffected = await db.ExecuteAsync(sqlQuery, new
            {
                entity.Name,
                entity.FileData,
                entity.ExpireDate,
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