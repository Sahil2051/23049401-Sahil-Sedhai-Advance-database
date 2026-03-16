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
        private sealed class SequenceMapping
        {
            public SequenceMapping(string tableName, string primaryKeyColumn, string sequenceName)
            {
                TableName = tableName;
                PrimaryKeyColumn = primaryKeyColumn;
                SequenceName = sequenceName;
            }

            public string TableName { get; }

            public string PrimaryKeyColumn { get; }

            public string SequenceName { get; }
        }

        private sealed class ForeignKeyMapping
        {
            public ForeignKeyMapping(string childTable, string childColumn, string parentTable, string parentColumn, string constraintName)
            {
                ChildTable = childTable;
                ChildColumn = childColumn;
                ParentTable = parentTable;
                ParentColumn = parentColumn;
                ConstraintName = constraintName;
            }

            public string ChildTable { get; }

            public string ChildColumn { get; }

            public string ParentTable { get; }

            public string ParentColumn { get; }

            public string ConstraintName { get; }
        }

        private static readonly object InitializationLock = new object();
        private static readonly SequenceMapping[] IdentitySequences =
        {
            new SequenceMapping("AppUser", "User_Id", "APPUSER_SEQ"),
            new SequenceMapping("Theater", "Theater_Id", "THEATER_SEQ"),
            new SequenceMapping("Hall", "Hall_Id", "HALL_SEQ"),
            new SequenceMapping("Movie", "Movie_Id", "MOVIE_SEQ"),
            new SequenceMapping("Show", "Show_Id", "SHOW_SEQ"),
            new SequenceMapping("Booking", "Booking_Id", "BOOKING_SEQ"),
            new SequenceMapping("Ticket", "Ticket_Id", "TICKET_SEQ")
        };
        private static readonly ForeignKeyMapping[] CascadeForeignKeys =
        {
            new ForeignKeyMapping("HALL", "THEATER_ID", "THEATER", "THEATER_ID", "FK_HALL_THEATER"),
            new ForeignKeyMapping("SHOW", "MOVIE_ID", "MOVIE", "MOVIE_ID", "FK_SHOW_MOVIE"),
            new ForeignKeyMapping("SHOW", "HALL_ID", "HALL", "HALL_ID", "FK_SHOW_HALL"),
            new ForeignKeyMapping("BOOKING", "USER_ID", "APPUSER", "USER_ID", "FK_BOOKING_APPUSER"),
            new ForeignKeyMapping("BOOKING", "SHOW_ID", "SHOW", "SHOW_ID", "FK_BOOKING_SHOW"),
            new ForeignKeyMapping("TICKET", "BOOKING_ID", "BOOKING", "BOOKING_ID", "FK_TICKET_BOOKING")
        };
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

                try
                {
                    EnsureCascadeDeleteConstraints();
                }
                catch (Exception ex)
                {
                    // Keep startup resilient; FK synchronization is best-effort safety.
                    Trace.TraceWarning("Foreign key synchronization was skipped: " + ex.Message);
                }

                try
                {
                    SynchronizeIdentitySequences();
                }
                catch (Exception ex)
                {
                    // Keep startup resilient; sequence sync is best-effort safety.
                    Trace.TraceWarning("Sequence synchronization was skipped: " + ex.Message);
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

        private static void SynchronizeIdentitySequences()
        {
            var cinemaConnection = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cinemaConnection) || HasPlaceholderCredentials(cinemaConnection))
            {
                return;
            }

            using (var connection = new OracleConnection(cinemaConnection))
            {
                connection.Open();

                for (var i = 0; i < IdentitySequences.Length; i++)
                {
                    SynchronizeSequence(connection, IdentitySequences[i]);
                }
            }
        }

        private static void EnsureCascadeDeleteConstraints()
        {
            var cinemaConnection = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cinemaConnection) || HasPlaceholderCredentials(cinemaConnection))
            {
                return;
            }

            using (var connection = new OracleConnection(cinemaConnection))
            {
                connection.Open();

                for (var i = 0; i < CascadeForeignKeys.Length; i++)
                {
                    EnsureCascadeForeignKey(connection, CascadeForeignKeys[i]);
                }
            }
        }

        private static void EnsureCascadeForeignKey(OracleConnection connection, ForeignKeyMapping mapping)
        {
            const string existingConstraintSql = @"SELECT uc.constraint_name, uc.delete_rule
                                                   FROM user_constraints uc
                                                   JOIN user_cons_columns ucc ON uc.constraint_name = ucc.constraint_name
                                                   JOIN user_constraints pk ON uc.r_constraint_name = pk.constraint_name
                                                   JOIN user_cons_columns pkc ON pk.constraint_name = pkc.constraint_name
                                                                             AND ucc.position = pkc.position
                                                   WHERE uc.constraint_type = 'R'
                                                     AND uc.table_name = :ChildTable
                                                     AND ucc.column_name = :ChildColumn
                                                     AND pk.table_name = :ParentTable
                                                     AND pkc.column_name = :ParentColumn";

            string currentConstraintName = null;
            string currentDeleteRule = null;

            using (var findConstraint = new OracleCommand(existingConstraintSql, connection))
            {
                findConstraint.BindByName = true;
                findConstraint.Parameters.Add(new OracleParameter(":ChildTable", mapping.ChildTable));
                findConstraint.Parameters.Add(new OracleParameter(":ChildColumn", mapping.ChildColumn));
                findConstraint.Parameters.Add(new OracleParameter(":ParentTable", mapping.ParentTable));
                findConstraint.Parameters.Add(new OracleParameter(":ParentColumn", mapping.ParentColumn));

                using (var reader = findConstraint.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        currentConstraintName = reader.GetString(0);
                        currentDeleteRule = reader.GetString(1);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(currentDeleteRule)
                && string.Equals(currentDeleteRule, "CASCADE", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(currentConstraintName))
            {
                var dropSql = "ALTER TABLE " + mapping.ChildTable + " DROP CONSTRAINT " + currentConstraintName;
                using (var dropCommand = new OracleCommand(dropSql, connection))
                {
                    dropCommand.ExecuteNonQuery();
                }
            }

            var addSql = "ALTER TABLE " + mapping.ChildTable
                         + " ADD CONSTRAINT " + mapping.ConstraintName
                         + " FOREIGN KEY (" + mapping.ChildColumn + ")"
                         + " REFERENCES " + mapping.ParentTable + " (" + mapping.ParentColumn + ")"
                         + " ON DELETE CASCADE";

            using (var addCommand = new OracleCommand(addSql, connection))
            {
                addCommand.ExecuteNonQuery();
            }
        }

        private static void SynchronizeSequence(OracleConnection connection, SequenceMapping mapping)
        {
            var maxIdSql = "SELECT NVL(MAX(" + mapping.PrimaryKeyColumn + "), 0) FROM " + mapping.TableName;
            var sequenceSql = "SELECT LAST_NUMBER FROM USER_SEQUENCES WHERE SEQUENCE_NAME = :SequenceName";

            decimal maxId;
            using (var maxIdCommand = new OracleCommand(maxIdSql, connection))
            {
                maxId = Convert.ToDecimal(maxIdCommand.ExecuteScalar() ?? 0m);
            }

            decimal nextValue;
            using (var sequenceCommand = new OracleCommand(sequenceSql, connection))
            {
                sequenceCommand.BindByName = true;
                sequenceCommand.Parameters.Add(new OracleParameter(":SequenceName", mapping.SequenceName));
                var result = sequenceCommand.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    return;
                }

                nextValue = Convert.ToDecimal(result);
            }

            if (nextValue > maxId)
            {
                return;
            }

            var incrementBy = Convert.ToInt64((maxId + 1m) - nextValue);
            if (incrementBy <= 0)
            {
                return;
            }

            var alterIncreaseSql = "ALTER SEQUENCE " + mapping.SequenceName + " INCREMENT BY " + incrementBy;
            var consumeSql = "SELECT " + mapping.SequenceName + ".NEXTVAL FROM dual";
            var alterResetSql = "ALTER SEQUENCE " + mapping.SequenceName + " INCREMENT BY 1";

            using (var alterIncrease = new OracleCommand(alterIncreaseSql, connection))
            {
                alterIncrease.ExecuteNonQuery();
            }

            using (var consume = new OracleCommand(consumeSql, connection))
            {
                consume.ExecuteScalar();
            }

            using (var alterReset = new OracleCommand(alterResetSql, connection))
            {
                alterReset.ExecuteNonQuery();
            }
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
