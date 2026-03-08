using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Hosting;

namespace CinemaTicketSystem.App_Code
{
    public class DatabaseHelper
    {
        private static readonly object InitializationLock = new object();
        private static bool _isInitialized;
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["CinemaDb"].ConnectionString;

        public static void EnsureDatabaseInitialized()
        {
            if (_isInitialized)
            {
                return;
            }

            lock (InitializationLock)
            {
                if (_isInitialized)
                {
                    return;
                }

                InitializeDatabaseFromScript();
                _isInitialized = true;
            }
        }

        public SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = OpenConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
            catch (SqlException)
            {
                return new DataTable();
            }
        }

        public int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            using (var connection = OpenConnection())
            using (var command = new SqlCommand(query, connection))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                return command.ExecuteNonQuery();
            }
        }

        private static void InitializeDatabaseFromScript()
        {
            var cinemaConnection = ConfigurationManager.ConnectionStrings["CinemaDb"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cinemaConnection))
            {
                throw new ConfigurationErrorsException("Connection string 'CinemaDb' is missing.");
            }

            var builder = new SqlConnectionStringBuilder(cinemaConnection);
            if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
            {
                throw new ConfigurationErrorsException("Connection string 'CinemaDb' must include Initial Catalog.");
            }

            var schemaPath = HostingEnvironment.MapPath("~/Sql/DatabaseSchema.sql");
            if (string.IsNullOrWhiteSpace(schemaPath) || !File.Exists(schemaPath))
            {
                throw new FileNotFoundException("Database schema script was not found.", schemaPath ?? "~/Sql/DatabaseSchema.sql");
            }

            var script = File.ReadAllText(schemaPath);
            builder.InitialCatalog = "master";

            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                foreach (var batch in Regex.Split(script, @"^\s*GO\s*(?:$|--.*$)", RegexOptions.Multiline | RegexOptions.IgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(batch))
                    {
                        continue;
                    }

                    using (var command = new SqlCommand(batch, connection))
                    {
                        command.CommandTimeout = 60;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
