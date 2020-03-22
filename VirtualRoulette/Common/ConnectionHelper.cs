using System;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace VirtualRoulette.Common
{
    public static class ConnectionHelper
    {
        public static async Task<T> ConnectAsync<T>(string connectionString, Func<MySqlConnection, Task<T>> f) //Using higher order function to reduce code duplications
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                return await f(conn);
            }
        }
    }
}
