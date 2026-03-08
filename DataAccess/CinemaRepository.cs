using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CinemaTicketSystem.DataAccess
{
    public class CinemaRepository
    {
        private readonly SqlDataAccess _dataAccess = new SqlDataAccess();

        private static readonly Dictionary<string, EntityConfig> EntityMap = new Dictionary<string, EntityConfig>(StringComparer.OrdinalIgnoreCase)
        {
            {
                "User",
                new EntityConfig("User", "User_Id", new[]
                {
                    "User_Name", "User_Email", "User_Contact_Number", "User_Address", "User_Registration_Date"
                })
            },
            {
                "Theater",
                new EntityConfig("Theater", "Theater_Id", new[]
                {
                    "Theater_Name", "Theater_City", "Theater_Location", "Theater_Contact_Number", "Theater_Total_Halls"
                })
            },
            {
                "Hall",
                new EntityConfig("Hall", "Hall_Id", new[]
                {
                    "Hall_Number", "Hall_Seating_Capacity", "Hall_Type", "Hall_Status", "Theater_Id"
                })
            },
            {
                "Movie",
                new EntityConfig("Movie", "Movie_Id", new[]
                {
                    "Movie_Title", "Movie_Duration", "Movie_Language", "Movie_Genre", "Movie_Release_Date"
                })
            },
            {
                "Show",
                new EntityConfig("Show", "Show_Id", new[]
                {
                    "Show_Date", "Show_Time", "Show_Rating", "Movie_Id", "Hall_Id"
                })
            },
            {
                "Booking",
                new EntityConfig("Booking", "Booking_Id", new[]
                {
                    "Booking_Date", "Booking_Status", "Total_Amount", "User_Id", "Show_Id"
                })
            },
            {
                "Ticket",
                new EntityConfig("Ticket", "Ticket_Id", new[]
                {
                    "Seat_Number", "Ticket_Status", "Ticket_Price", "Booking_Id"
                })
            }
        };

        public DataTable GetAll(string entityName)
        {
            var config = GetConfig(entityName);
            var sql = $"SELECT * FROM {QuoteName(config.TableName)} ORDER BY {QuoteName(config.PrimaryKey)} DESC";
            return _dataAccess.ExecuteDataTable(sql);
        }

        public DataRow GetById(string entityName, int id)
        {
            var config = GetConfig(entityName);
            var sql = $"SELECT * FROM {QuoteName(config.TableName)} WHERE {QuoteName(config.PrimaryKey)} = @Id";
            var table = _dataAccess.ExecuteDataTable(sql, new[] { new SqlParameter("@Id", id) });
            return table.Rows.Count == 0 ? null : table.Rows[0];
        }

        public int GetCount(string entityName)
        {
            var config = GetConfig(entityName);
            var sql = $"SELECT COUNT(1) AS TotalCount FROM {QuoteName(config.TableName)}";
            var table = _dataAccess.ExecuteDataTable(sql);
            return table.Rows.Count == 0 ? 0 : Convert.ToInt32(table.Rows[0]["TotalCount"]);
        }

        public void Insert(string entityName, IDictionary<string, object> values)
        {
            var config = GetConfig(entityName);
            var filteredValues = FilterValues(config, values);

            var columns = string.Join(", ", filteredValues.Keys.Select(QuoteName));
            var parameters = string.Join(", ", filteredValues.Keys.Select(column => $"@{column}"));
            var sql = $"INSERT INTO {QuoteName(config.TableName)} ({columns}) VALUES ({parameters})";

            _dataAccess.ExecuteNonQuery(sql, BuildParameters(filteredValues));
        }

        public void Update(string entityName, int id, IDictionary<string, object> values)
        {
            var config = GetConfig(entityName);
            var filteredValues = FilterValues(config, values);

            var setClause = string.Join(", ", filteredValues.Keys.Select(column => $"{QuoteName(column)} = @{column}"));
            var sql = $"UPDATE {QuoteName(config.TableName)} SET {setClause} WHERE {QuoteName(config.PrimaryKey)} = @Id";

            var parameters = BuildParameters(filteredValues);
            parameters.Add(new SqlParameter("@Id", id));

            _dataAccess.ExecuteNonQuery(sql, parameters);
        }

        public void Delete(string entityName, int id)
        {
            var config = GetConfig(entityName);
            var sql = $"DELETE FROM {QuoteName(config.TableName)} WHERE {QuoteName(config.PrimaryKey)} = @Id";
            _dataAccess.ExecuteNonQuery(sql, new[] { new SqlParameter("@Id", id) });
        }

        private static EntityConfig GetConfig(string entityName)
        {
            if (EntityMap.TryGetValue(entityName, out var config))
            {
                return config;
            }

            throw new InvalidOperationException($"Unsupported entity: {entityName}");
        }

        private static Dictionary<string, object> FilterValues(EntityConfig config, IDictionary<string, object> values)
        {
            var filtered = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var column in config.EditableColumns)
            {
                if (values.TryGetValue(column, out var value))
                {
                    filtered[column] = value ?? DBNull.Value;
                }
            }

            return filtered;
        }

        private static List<SqlParameter> BuildParameters(IDictionary<string, object> values)
        {
            var parameters = new List<SqlParameter>();
            foreach (var item in values)
            {
                parameters.Add(new SqlParameter($"@{item.Key}", item.Value ?? DBNull.Value));
            }

            return parameters;
        }

        private static string QuoteName(string name)
        {
            return $"[{name}]";
        }

        private sealed class EntityConfig
        {
            public EntityConfig(string tableName, string primaryKey, IEnumerable<string> editableColumns)
            {
                TableName = tableName;
                PrimaryKey = primaryKey;
                EditableColumns = editableColumns.ToList();
            }

            public string TableName { get; }

            public string PrimaryKey { get; }

            public IReadOnlyList<string> EditableColumns { get; }
        }
    }
}
