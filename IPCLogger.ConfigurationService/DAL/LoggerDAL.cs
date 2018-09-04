using IPCLogger.ConfigurationService.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace IPCLogger.ConfigurationService.DAL
{
    internal class LoggerDAL : BaseDAL<LoggerDAL>
    {
        public List<LoggerModel> GetLoggers(bool includeHidden)
        {
            List<LoggerModel> loggers = new List<LoggerModel>();

            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT *
FROM T_LOGGERS
WHERE visible = 1 OR @include_hidden = 1";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        loggers.Add(new LoggerModel
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            ApplicationName = reader["application_name"].ToString(),
                            Description = reader["description"].ToString(),
                            ConfigurationFile = reader["configuration_file"].ToString(),
                            Visible = Convert.ToBoolean(reader["visible"])
                        });
                    }
                }
            }

            return loggers;
        }

        private void CreateTables(SQLiteCommand command)
        {
            //Create T_APPLICATIONS table
            command.CommandText = @"
CREATE TABLE T_LOGGERS
(
    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    application_name TEXT(100) NOT NULL,
    description TEXT(160),
    configuration_file TEXT(260) NOT NULL,
    visible BOOLEAN DEFAULT 1 NOT NULL
)";
            command.ExecuteNonQuery();
        }

        private void CreateTablesData(SQLiteCommand command)
        {
        }

        protected override void CreateDbStructure()
        {
            using (SQLiteTransaction transaction = Connection.BeginTransaction())
            {
                using (SQLiteCommand command = new SQLiteCommand(Connection))
                {
                    //Create tables structure
                    CreateTables(command);

                    //Create tables data
                    CreateTablesData(command);

                    //Commit
                    transaction.Commit();
                }
            }
        }
    }
}
