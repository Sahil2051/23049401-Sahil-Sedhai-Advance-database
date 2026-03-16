using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using CinemaTicketSystem.Helpers;
using System.Text.RegularExpressions;

namespace CinemaTicketSystem.DataAccess
{
    public class SqlDataAccess
    {
        public DataTable ExecuteDataTable(string sql, IEnumerable<OracleParameter> parameters = null)
        {
            using (var connection = ConnectionHelper.CreateConnection())
            using (var command = new OracleCommand(NormalizeSql(sql), connection))
            {
                command.BindByName = true;

                if (parameters != null)
                {
                    command.Parameters.AddRange(ToArray(parameters));
                }

                using (var adapter = new OracleDataAdapter(command))
                {
                    var table = new DataTable();
                    try
                    {
                        adapter.Fill(table);
                    }
                    catch (OracleException)
                    {
                        return new DataTable();
                    }

                    return table;
                }
            }
        }

        public int ExecuteNonQuery(string sql, IEnumerable<OracleParameter> parameters = null)
        {
            using (var connection = ConnectionHelper.CreateConnection())
            using (var command = new OracleCommand(NormalizeSql(sql), connection))
            {
                command.BindByName = true;

                if (parameters != null)
                {
                    command.Parameters.AddRange(ToArray(parameters));
                }

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        private static OracleParameter[] ToArray(IEnumerable<OracleParameter> parameters)
        {
            var list = new List<OracleParameter>();
            foreach (var parameter in parameters)
            {
                if (parameter == null)
                {
                    continue;
                }

                var parameterName = parameter.ParameterName ?? string.Empty;
                parameterName = parameterName.Trim();
                parameterName = parameterName.TrimStart('@', ':');
                parameter.ParameterName = ":" + parameterName;
                if (parameter.Value == null)
                {
                    parameter.Value = DBNull.Value;
                }

                list.Add(parameter);
            }

            return list.ToArray();
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
