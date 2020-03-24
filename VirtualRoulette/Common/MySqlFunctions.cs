using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace VirtualRoulette.Common
{
    public static class MySqlFunctions
    {
        public static async Task<T> ConnectAsync<T>(string connectionString, Func<MySqlConnection, Task<T>> f) //Using higher order function to reduce code duplications
        {
            using (var conn = new MySqlConnection(connectionString)) // Don't need extra try finally
            {
                await conn.OpenAsync();
                return await f(conn);
            }
        }
        
        public static async Task ConnectAsync(string connectionString, Func<MySqlConnection, Task> f)
        {
            using (var conn = new MySqlConnection(connectionString)) // Don't need extra try finally
            {
                await conn.OpenAsync();
                await f(conn);
            }
        }

        public static async Task<TResult> QueryOneAsync<TResult>(this MySqlCommand self, Func<DbDataReader, TResult> mapper)
        {
            var reader = await self.ExecuteReaderAsync();

            if (!reader.HasRows) return default;

            await reader.ReadAsync();

            return mapper(reader);
        }

        public static async Task<List<TResult>> QueryAsync<TResult>(this MySqlCommand self, Func<DbDataReader, TResult> mapper)
        {
            var reader = await self.ExecuteReaderAsync();

            if (!reader.HasRows) return default;

            var result= new List<TResult>();

            while (await reader.ReadAsync())
            {
                result.Add(mapper(reader));
            }

            return result;
        }
    }
}
