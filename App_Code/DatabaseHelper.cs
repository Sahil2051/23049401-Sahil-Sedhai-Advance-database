using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Hosting;

namespace CinemaTicketSystem.App_Code
{
    public class DatabaseHelper
    {
        private static readonly object InitializationLock = new object();
        private static bool _isInitialized;
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;

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

                try
                {
                    InitializeDatabaseFromScript();
                }
                catch (Exception ex)
                {
                    // Do not block site startup if Oracle auth or privileges are not ready yet.
                    Trace.TraceWarning("Database initialization was skipped: " + ex.Message);
                }

                _isInitialized = true;
            }
        }

        public OracleConnection OpenConnection()
        {
            var connection = new OracleConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public DataTable ExecuteQuery(string query, params OracleParameter[] parameters)
        {
            try
            {
                using (var connection = OpenConnection())
                using (var command = new OracleCommand(NormalizeSql(query), connection))
                {
                    command.BindByName = true;

                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(NormalizeParameters(parameters));
                    }

                    using (var adapter = new OracleDataAdapter(command))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
            catch (OracleException)
            {
                return new DataTable();
            }
        }

        public int ExecuteNonQuery(string query, params OracleParameter[] parameters)
        {
            using (var connection = OpenConnection())
            using (var command = new OracleCommand(NormalizeSql(query), connection))
            {
                command.BindByName = true;

                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(NormalizeParameters(parameters));
                }

                return command.ExecuteNonQuery();
            }
        }

        private static void InitializeDatabaseFromScript()
        {
            var cinemaConnection = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cinemaConnection))
            {
                throw new ConfigurationErrorsException("Connection string 'OracleConnection' is missing.");
            }

            if (HasPlaceholderCredentials(cinemaConnection))
            {
                // Skip initialization when default placeholder credentials are still configured.
                return;
            }

            var schemaPath = HostingEnvironment.MapPath("~/Sql/DatabaseSchema.sql");
            if (string.IsNullOrWhiteSpace(schemaPath) || !File.Exists(schemaPath))
            {
                throw new FileNotFoundException("Database schema script was not found.", schemaPath ?? "~/Sql/DatabaseSchema.sql");
            }

            var script = File.ReadAllText(schemaPath);

            using (var connection = new OracleConnection(cinemaConnection))
            {
                connection.Open();

                foreach (var batch in Regex.Split(script, @"^\s*/\s*$", RegexOptions.Multiline))
                {
                    if (string.IsNullOrWhiteSpace(batch))
                    {
                        continue;
                    }

                    using (var command = new OracleCommand(batch, connection))
                    {
                        command.BindByName = true;
                        command.CommandTimeout = 60;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static bool HasPlaceholderCredentials(string connectionString)
        {
            var builder = new OracleConnectionStringBuilder(connectionString);
            var password = builder.Password ?? string.Empty;
            return string.IsNullOrWhiteSpace(password)
                   || string.Equals(password, "yourpassword", StringComparison.OrdinalIgnoreCase);
        }

        private static OracleParameter[] NormalizeParameters(OracleParameter[] parameters)
        {
            var normalized = new System.Collections.Generic.List<OracleParameter>();
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (parameter == null)
                {
                    continue;
                }

                var parameterName = parameter.ParameterName ?? string.Empty;
                parameterName = parameterName.Trim().TrimStart('@', ':');
                parameter.ParameterName = ":" + parameterName;
                if (parameter.Value == null)
                {
                    parameter.Value = DBNull.Value;
                }

                normalized.Add(parameter);
            }

            return normalized.ToArray();
        }

        private static string NormalizeSql(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                return sql;
            }

            var normalized = Regex.Replace(sql, @"\[(?<name>[^\]]+)\]", "${name}");
            normalized = Regex.Replace(normalized, @"@(?<name>[A-Za-z0-9_]+)", ":${name}");
            return normalized;
        }
    }
}
