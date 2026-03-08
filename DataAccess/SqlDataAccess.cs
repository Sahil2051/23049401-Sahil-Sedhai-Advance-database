using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CinemaTicketSystem.Helpers;

namespace CinemaTicketSystem.DataAccess
{
    public class SqlDataAccess
    {
        public DataTable ExecuteDataTable(string sql, IEnumerable<SqlParameter> parameters = null)
        {
            using (var connection = ConnectionHelper.CreateConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(ToArray(parameters));
                }

                using (var adapter = new SqlDataAdapter(command))
                {
                    var table = new DataTable();
                    try
                    {
                        adapter.Fill(table);
                    }
                    catch (SqlException)
                    {
                        return new DataTable();
                    }

                    return table;
                }
            }
        }

        public int ExecuteNonQuery(string sql, IEnumerable<SqlParameter> parameters = null)
        {
            using (var connection = ConnectionHelper.CreateConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(ToArray(parameters));
                }

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        private static SqlParameter[] ToArray(IEnumerable<SqlParameter> parameters)
        {
            var list = new List<SqlParameter>();
            foreach (var parameter in parameters)
            {
                list.Add(parameter);
            }

            return list.ToArray();
        }
    }
}
