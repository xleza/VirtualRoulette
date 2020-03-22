using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using VirtualRoulette.Domain;
using VirtualRoulette.Common;
using static VirtualRoulette.Common.ConnectionHelper;

namespace VirtualRoulette.Persistence
{
    public sealed class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;

        public UsersRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<User> Get(int id) =>
            ConnectAsync(_connectionString,
                async con =>
                {
                    var command = new MySqlCommand("SELECT id, username, password, firstName, lastName FROM users WHERE id = @id LIMIT 1", con);
                    command.Parameters.AddWithValue("@id", id);
                    var reader = await command.ExecuteReaderAsync();

                    if (!reader.HasRows) return null;

                    await reader.ReadAsync();

                    return new User
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("username"),
                        Password = reader.GetString("password"),
                        FirstName = reader.GetString("FirstName"),
                        LastName = reader.GetString("LastName")
                    };
                });

        public Task<User> Get(string username) =>
            ConnectAsync(_connectionString,
                async con =>
                {
                    var command = new MySqlCommand("SELECT id, username, password, firstName, lastName FROM users WHERE username = @username LIMIT 1", con);
                    command.Parameters.AddWithValue("@username", username);
                    var reader = await command.ExecuteReaderAsync();

                    if (!reader.HasRows) return null;

                    await reader.ReadAsync();

                    return new User
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("username"),
                        Password = reader.GetString("password"),
                        FirstName = reader.GetString("FirstName"),
                        LastName = reader.GetString("LastName")
                    };
                });
    }
}
