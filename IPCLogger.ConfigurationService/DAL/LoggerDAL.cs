using IPCLogger.ConfigurationService.Entities.DTO;
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
WHERE
    deleted = 0 AND
    (visible = 1 OR @include_hidden = 1)";

                command.Parameters.Add(new SQLiteParameter("@include_hidden", includeHidden));

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

        public int Create(LoggerRegDTO dto)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
INSERT INTO T_LOGGERS
    (application_name, description, configuration_file)
VALUES
    (@application_name, @description, @configuration_file);
SELECT last_insert_rowid()";

                command.Parameters.Add(new SQLiteParameter("@application_name", dto.ApplicationName));
                command.Parameters.Add(new SQLiteParameter("@description", StringOrDBNull(dto.Description)));
                command.Parameters.Add(new SQLiteParameter("@configuration_file", dto.ConfigurationFile));
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int Update(LoggerRegDTO dto)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
UPDATE T_LOGGERS
SET
     application_name = @application_name
    ,description = @description
    ,configuration_file = @configuration_file
WHERE id = @logger_id";

                command.Parameters.Add(new SQLiteParameter("@logger_id", dto.Id));
                command.Parameters.Add(new SQLiteParameter("@application_name", dto.ApplicationName));
                command.Parameters.Add(new SQLiteParameter("@description", StringOrDBNull(dto.Description)));
                command.Parameters.Add(new SQLiteParameter("@configuration_file", dto.ConfigurationFile));
                command.ExecuteNonQuery();

                return dto.Id;
            }
        }

        public void Delete(int loggerId)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
UPDATE T_LOGGERS
SET deleted = 1
WHERE id = @logger_id";

                command.Parameters.Add(new SQLiteParameter("@logger_id", loggerId));
                command.ExecuteNonQuery();
            }
        }

        public void ChangeVisibility(int loggerId, bool visible)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
UPDATE T_LOGGERS
SET visible = @visible
WHERE id = @logger_id";

                command.Parameters.Add(new SQLiteParameter("@logger_id", loggerId));
                command.Parameters.Add(new SQLiteParameter("@visible", visible));
                command.ExecuteNonQuery();
            }
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
    visible BOOLEAN DEFAULT 1 NOT NULL,
    deleted BOOLEAN DEFAULT 0 NOT NULL
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