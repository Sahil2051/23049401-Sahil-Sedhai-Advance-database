using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace CinemaTicketSystem.Helpers
{
    public static class ConnectionHelper
    {
        private const string ConnectionName = "OracleConnection";

        public static OracleConnection CreateConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[ConnectionName]?.ConnectionString;
            return new OracleConnection(connectionString);
        }
    }
}
