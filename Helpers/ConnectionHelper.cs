using System.Configuration;
using System.Data.SqlClient;

namespace CinemaTicketSystem.Helpers
{
    public static class ConnectionHelper
    {
        private const string ConnectionName = "CinemaDb";

        public static SqlConnection CreateConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[ConnectionName]?.ConnectionString;
            return new SqlConnection(connectionString);
        }
    }
}
