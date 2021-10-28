using System;
using System.Data.SqlClient;
using TicketManagementSystem.Core;

namespace TicketManagementSystem.Infrastructure
{
    public class UserRepository : IDisposable, IUserRepository
    {
        public User GetUser(string username)
        {
            /*// Assume this method does not need to change and is connected to a database with users populated.
            try
            {
                var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["database"].ConnectionString;

                string sql = "SELECT TOP 1 FROM Users u WHERE u.Username == @p1";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand(sql, connection)
                    {
                        CommandType = System.Data.CommandType.Text,
                    };

                    command.Parameters.Add("@p1", System.Data.SqlDbType.NVarChar).Value = username;

                    return (User)command.ExecuteScalar();
                }
            }
            catch (Exception)
            {
                return null;
            }*/

            return new User()
            {
                FirstName = "John",
                LastName = "Pettersson",
                Username = "JP"
            };
        }

        public User GetAccountManager()
        {
            // Assume this method does not need to change.
            return new User()
            {
                FirstName = "Sarah",
                LastName = "Connor",
                Username = "SC"
            };
        }

        public void Dispose()
        {
            // Free up resources
        }
    }
}
