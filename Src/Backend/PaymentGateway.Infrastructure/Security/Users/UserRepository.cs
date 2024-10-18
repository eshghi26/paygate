using Dapper;
using PaymentGateway.Infrastructure.Helper;
using System.Data;
using Common.Helper.Extension;
using PaymentGateway.Domain.Base;
using PaymentGateway.Domain.Security.Users;

namespace PaymentGateway.Infrastructure.Security.Users
{
    public class UserRepository : IUserRepository
    {
        #region DataMember
        private const string TableName = @"""Security"".""Users""";
        #endregion

        #region Fetch
        public async Task<(List<User> Data, int TotalCount)> GetList(FetchOption filter)
        {
            using var db = await new DbEntityObject().OpenConnection();

            #region Set Where Param
            var prams = new DynamicParameters();

            var whereQuery = @"WHERE ""IsDeleted"" = false ";

            if (filter.Filters != null && filter.Filters.Any())
            {
                var filters = filter.Filters.Select(c => new FilterItem
                {
                    FieldName = c.FieldName.ToPascalCase(),
                    Values = c.Values,
                    Operator = c.Operator
                }).ToArray();

                #region Id
                if (filters.Any(c => c.FieldName == "Id"))
                {
                    var obj = filters.First(c => c.FieldName == "Id");
                    var rec = QueryBuilder.CreateFieldQuery(obj, DbType.Int32);
                    if (rec is { query: { }, values: { } })
                    {
                        whereQuery += rec.query;

                        foreach (var item in rec.values)
                        {
                            if (rec.setDbType)
                                prams.Add(item.Key, item.Value, DbType.Int32);
                            else
                            {
                                prams.Add(item.Key, item.Value);
                            }
                        }
                    }
                }
                #endregion

                #region Username
                if (filters.Any(c => c.FieldName == "Username"))
                {
                    var obj = filters.First(c => c.FieldName == "Username");
                    var rec = QueryBuilder.CreateFieldQuery(obj, DbType.String);
                    if (rec is { query: { }, values: { } })
                    {
                        whereQuery += rec.query;

                        foreach (var item in rec.values)
                        {
                            if (rec.setDbType)
                                prams.Add(item.Key, item.Value, DbType.String);
                            else
                            {
                                prams.Add(item.Key, item.Value);
                            }
                        }
                    }
                }
                #endregion

                #region FirstName
                if (filters.Any(c => c.FieldName == "FirstName"))
                {
                    var obj = filters.First(c => c.FieldName == "FirstName");
                    var rec = QueryBuilder.CreateFieldQuery(obj, DbType.String);
                    if (rec is { query: { }, values: { } })
                    {
                        whereQuery += rec.query;

                        foreach (var item in rec.values)
                        {
                            if (rec.setDbType)
                                prams.Add(item.Key, item.Value, DbType.String);
                            else
                            {
                                prams.Add(item.Key, item.Value);
                            }
                        }
                    }
                }
                #endregion

                #region LastName
                if (filters.Any(c => c.FieldName == "LastName"))
                {
                    var obj = filters.First(c => c.FieldName == "LastName");
                    var rec = QueryBuilder.CreateFieldQuery(obj, DbType.String);
                    if (rec is { query: { }, values: { } })
                    {
                        whereQuery += rec.query;

                        foreach (var item in rec.values)
                        {
                            if (rec.setDbType)
                                prams.Add(item.Key, item.Value, DbType.String);
                            else
                            {
                                prams.Add(item.Key, item.Value);
                            }
                        }
                    }
                }
                #endregion

                #region NickName
                if (filters.Any(c => c.FieldName == "NickName"))
                {
                    var obj = filters.First(c => c.FieldName == "NickName");
                    var rec = QueryBuilder.CreateFieldQuery(obj, DbType.String);
                    if (rec is { query: { }, values: { } })
                    {
                        whereQuery += rec.query;

                        foreach (var item in rec.values)
                        {
                            if (rec.setDbType)
                                prams.Add(item.Key, item.Value, DbType.String);
                            else
                            {
                                prams.Add(item.Key, item.Value);
                            }
                        }
                    }
                }
                #endregion

                #region Email
                if (filters.Any(c => c.FieldName == "Email"))
                {
                    var obj = filters.First(c => c.FieldName == "Email");
                    var rec = QueryBuilder.CreateFieldQuery(obj, DbType.String);
                    if (rec is { query: { }, values: { } })
                    {
                        whereQuery += rec.query;

                        foreach (var item in rec.values)
                        {
                            if (rec.setDbType)
                                prams.Add(item.Key, item.Value, DbType.String);
                            else
                            {
                                prams.Add(item.Key, item.Value);
                            }
                        }
                    }
                }
                #endregion

                #region MobileNumber
                if (filters.Any(c => c.FieldName == "MobileNumber"))
                {
                    var obj = filters.First(c => c.FieldName == "MobileNumber");
                    var rec = QueryBuilder.CreateFieldQuery(obj, DbType.String);
                    if (rec is { query: { }, values: { } })
                    {
                        whereQuery += rec.query;

                        foreach (var item in rec.values)
                        {
                            if (rec.setDbType)
                                prams.Add(item.Key, item.Value, DbType.String);
                            else
                            {
                                prams.Add(item.Key, item.Value);
                            }
                        }
                    }
                }
                #endregion

                #region IsActive
                if (filters.Any(c => c.FieldName == "IsActive"))
                {
                    var obj = filters.First(c => c.FieldName == "IsActive");
                    var rec = QueryBuilder.CreateFieldQuery(obj, DbType.Boolean);
                    if (rec is { query: { }, values: { } })
                    {
                        whereQuery += rec.query;

                        foreach (var item in rec.values)
                        {
                            if (rec.setDbType)
                                prams.Add(item.Key, item.Value, DbType.Boolean);
                            else
                            {
                                prams.Add(item.Key, item.Value);
                            }
                        }
                    }
                }
                #endregion

                #region IsBan
                if (filters.Any(c => c.FieldName == "IsBan"))
                {
                    var obj = filters.First(c => c.FieldName == "IsBan");
                    var rec = QueryBuilder.CreateFieldQuery(obj, DbType.Boolean);
                    if (rec is { query: { }, values: { } })
                    {
                        whereQuery += rec.query;

                        foreach (var item in rec.values)
                        {
                            if (rec.setDbType)
                                prams.Add(item.Key, item.Value, DbType.Boolean);
                            else
                            {
                                prams.Add(item.Key, item.Value);
                            }
                        }
                    }
                }
                #endregion

                #region CreateOn
                if (filters.Any(c => c.FieldName == "CreateOn"))
                {
                    var obj = filters.First(c => c.FieldName == "CreateOn");
                    var rec = QueryBuilder.CreateFieldQuery(obj, DbType.DateTime);
                    if (rec is { query: { }, values: { } })
                    {
                        whereQuery += rec.query;

                        foreach (var item in rec.values)
                        {
                            if (rec.setDbType)
                                prams.Add(item.Key, item.Value, DbType.DateTime);
                            else
                            {
                                prams.Add(item.Key, item.Value);
                            }
                        }
                    }
                }
                #endregion

                #region Roles
                if (filters.Any(c => c.FieldName == "Roles"))
                {
                    var obj = filters.First(c => c.FieldName == "Roles");
                    if (obj.Values != null &&
                        obj.Values.Any())
                    {
                        var arrayVal = QueryBuilder.ParseStringToArrayObject(obj.Values, DbType.Int32);
                        if (arrayVal != null)
                        {
                            whereQuery += $@" AND ""Id"" = ANY (SELECT ""EmployeeId"" FROM ""Security"".""EmployeeRoles"" WHERE ""RoleId"" = ANY(@Roles))";
                            prams.Add("@Roles", arrayVal);
                        }
                    }
                }
                #endregion
            }
            #endregion

            #region Set SortOption
            string sortOption;
            if (filter.SortOptions != null && filter.SortOptions.Any())
            {
                var items = filter.SortOptions.Select(c => $@"""{c.FieldName.ToPascalCase()}"" {c.Direction}");
                sortOption = $@"ORDER BY {string.Join(", ", items)} ";
            }
            else
            {
                sortOption = @"ORDER BY ""Id"" DESC ";
            }
            #endregion

            #region Set Pagination
            var pagingStr = string.Empty;

            if (filter is { PageSize: { }, Page: { } })
            {
                var pageIndex = filter.Page.Value;

                var skip = 0;
                if (pageIndex > 0)
                {
                    skip = pageIndex * filter.PageSize.Value;
                }

                pagingStr = $@"OFFSET @Skip ROWS FETCH NEXT {filter.PageSize.Value} ROWS ONLY";
                prams.Add("Skip", skip);
            }
            #endregion

            #region Sql Query
            var sqlQuery = $@"SELECT *
                                  FROM {TableName}
                                  {whereQuery}
                                  {sortOption}
                                  {pagingStr};

                                  Select COUNT(1) 
                                  FROM {TableName}
                                  {whereQuery}";
            #endregion

            #region Get Data From Db
            await using var multiData = await db.QueryMultipleAsync(sqlQuery, prams);

            var data = (await multiData.ReadAsync<User>()).ToList();
            var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();

            return (data, totalCount);
            #endregion
        }

        public async Task<User?> GetById(long id)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<User>($@"Select * From {TableName} WHERE ""Id"" = @id And ""IsDeleted"" = false", new { id });

            return result.SingleOrDefault();
        }

        public async Task<User?> Login(string userName, string password)
        {
            using var db = await new DbEntityObject().OpenConnection();
            userName = userName.ToLower();
            var result = await db.QueryAsync<User>($@"Select * From {TableName} WHERE lower(""Username"") = @userName AND ""Password"" = @password And ""IsDeleted"" = false",
                new { userName, password });

            return result.SingleOrDefault();
        }

        public async Task<User?> GetByRefreshTokenSerial(string refreshTokenSerial)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<User>($@"Select * From {TableName} 
                                                                           WHERE ""RefreshTokenSerial"" = @refreshTokenSerial",
                new { refreshTokenSerial });

            return result.SingleOrDefault();
        }

        public async Task<bool> UserNameIsExist(string userName, int id)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<User>($@"Select * From {TableName} WHERE lower(""Username"") = lower(@userName)", new { userName });

            var record = result.FirstOrDefault();

            if (record == null)
                return false;

            if (record.Id == id)
                return false;

            return true;
        }

        public async Task<bool> EmailIsExist(string email, int id)
        {
            using var db = await new DbEntityObject().OpenConnection();
            var result = await db.QueryAsync<User>($@"Select * From {TableName} WHERE lower(""Email"") = lower(@email)", new { email });

            var record = result.FirstOrDefault();

            if (record == null)
                return false;

            if (record.Id == id)
                return false;

            return true;
        }
        #endregion

        #region Insert
        public async Task<int> Insert(User entity)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var prams = new DynamicParameters();
            prams.Add("@Username", entity.Username);
            prams.Add("@Password", entity.Password);
            prams.Add("@FirstName", entity.FirstName);
            prams.Add("@LastName", entity.LastName);
            prams.Add("@NickName", entity.NickName);
            prams.Add("@Email", entity.Email);
            prams.Add("@MobileNumber", entity.MobileNumber);
            prams.Add("@ProfilePicture", entity.ProfilePicture);
            prams.Add("@IsActive", entity.IsActive);
            prams.Add("@IsBan", entity.IsBan);
            prams.Add("@CreateOn", DateTime.Now);
            prams.Add("@IsDeleted", false);

            var entityId = (await db.QueryAsync<int>(
                $@"INSERT INTO {TableName} 
                               (
                                       ""Username""
                                      ,""Password""
                                      ,""FirstName""
                                      ,""LastName""
                                      ,""NickName""
                                      ,""Email""
                                      ,""MobileNumber""
                                      ,""ProfilePicture""
                                      ,""IsActive""
                                      ,""IsBan""
                                      ,""CreateOn""
                                      ,""IsDeleted""
                               )
                               VALUES
                               (
                                       @Username
                                      ,@Password
                                      ,@FirstName
                                      ,@LastName
                                      ,@NickName
                                      ,@Email
                                      ,@MobileNumber
                                      ,@ProfilePicture
                                      ,@IsActive
                                      ,@IsBan
                                      ,@CreateOn
                                      ,@IsDeleted
                               );
                               SELECT currval('""Security"".""Employees_Id_seq""');", prams)).SingleOrDefault();

            return entityId;
        }
        #endregion

        #region Update
        public async Task<bool> Update(User entity)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        ""Username"" = @Username
                                       ,""FirstName"" = @FirstName
                                       ,""LastName"" = @LastName
                                       ,""NickName"" = @NickName
                                       ,""Email"" = @Email
                                       ,""MobileNumber"" = @MobileNumber
                                       ,""ProfilePicture"" = @ProfilePicture
                                       ,""IsActive"" = @IsActive
                                       ,""IsBan"" = @IsBan
                                   WHERE ""Id"" = @Id";

            var rowsAffected = await db.ExecuteAsync(sqlQuery, new
            {
                entity.Username,
                entity.FirstName,
                entity.LastName,
                entity.NickName,
                entity.Email,
                entity.MobileNumber,
                entity.ProfilePicture,
                entity.IsActive,
                entity.IsBan,
                entity.Id
            });

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateRefreshToken(int id, string? refreshTokenSerial, DateTime? refreshTokenExpiryDate)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        ""RefreshTokenSerial"" = @refreshTokenSerial
                                       ,""RefreshTokenExpiryDate"" = @refreshTokenExpiryDate
                                   WHERE ""Id"" = @id";

            var rowsAffected = await db.ExecuteAsync(sqlQuery, new
            {
                refreshTokenSerial,
                refreshTokenExpiryDate,
                id
            });

            return rowsAffected > 0;
        }

        public async Task<bool> UpdatePassword(int id, string password)
        {
            using var db = await new DbEntityObject().OpenConnection();

            var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        ""Password"" = @password
                                   WHERE ""Id"" = @id";

            var rowsAffected = await db.ExecuteAsync(sqlQuery, new
            {
                password,
                id
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