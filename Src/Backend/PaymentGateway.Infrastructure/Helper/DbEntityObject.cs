using Npgsql;
using System.Data;
using Common.Helper.Helper;
using Microsoft.Extensions.Configuration;

namespace PaymentGateway.Infrastructure.Helper
{
    public class DbEntityObject
    {
        public async Task<IDbConnection> OpenConnection()
        {
            var conn = new NpgsqlConnection(ConfigurationHelper.Current?.GetConnectionString("PgDbContext"));
            await conn.OpenAsync();
            return conn;
        }

        public IDbConnection OpenConnectionSync()
        {
            var conn = new NpgsqlConnection(ConfigurationHelper.Current?.GetConnectionString("PgDbContext"));
            conn.Open();
            return conn;
        }
    }
}
