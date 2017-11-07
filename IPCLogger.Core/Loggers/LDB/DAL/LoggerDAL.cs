using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace IPCLogger.Core.Loggers.LDB.DAL
{
    class LoggerDAL
    {
        private string _connectionString;

        public LoggerDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public List<ColumnInfo> GetTableColumns(string tableName)
        {
            using (SqlConnection connection = OpenConnection())
            {
                string sqlQuery = string.Format(
                    @"
SELECT 
     c.name
    ,c.system_type_id
    ,c.is_identity
    ,c.is_nullable
FROM [{0}].sys.columns c (NOLOCK)
WHERE object_id = OBJECT_ID('[{0}].dbo.[{1}]') AND c.is_computed = 0
", connection.Database, tableName);
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<ColumnInfo> result = new List<ColumnInfo>();
                        while (reader.Read())
                        {
                            ColumnInfo info = new ColumnInfo
                            {
                                Name = (string) reader["name"],
                                TypeId = (byte) reader["system_type_id"],
                                IsIdentity = (bool) reader["is_identity"],
                                IsNullable = (bool) reader["is_nullable"]
                            };
                            result.Add(info);
                        }
                        return result;
                    }
                }
            }
        }

        public void WriteLog(string tableName, DataRow[] rows)
        {
            using (SqlConnection connection = OpenConnection())
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, null))
                {
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.BatchSize = rows.Length;
                    bulkCopy.WriteToServer(rows);
                }
            }
        }
    }
}