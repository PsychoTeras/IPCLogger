using IPCLogger.ConfigurationService.Common.Exceptions;
using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.ConfigurationService.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace IPCLogger.ConfigurationService.DAL
{
    internal class ApplicationDAL : BaseDAL<ApplicationDAL>
    {
        private ApplicationModel ApplicationModelFromReader(SQLiteDataReader reader)
        {
            return new ApplicationModel
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = reader["name"].ToString(),
                Description = reader["description"].ToString(),
                ConfigurationFile = reader["configuration_file"].ToString(),
                Visible = Convert.ToBoolean(reader["visible"])
            };
        }

        public ApplicationModel GetApplication(int applicationId)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT *
FROM T_APPLICATIONS
WHERE
    id = @applicationId AND
    deleted = 0 AND
    visible = 1";

                command.Parameters.Add(new SQLiteParameter("@applicationId", applicationId));

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ApplicationModelFromReader(reader);
                    }
                }
            }

            throw new InvalidRequestException();
        }

        public List<ApplicationModel> GetApplications(bool includeHidden = false)
        {
            List<ApplicationModel> applications = new List<ApplicationModel>();

            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT *
FROM T_APPLICATIONS
WHERE
    deleted = 0 AND
    (visible = 1 OR @include_hidden = 1)";

                command.Parameters.Add(new SQLiteParameter("@include_hidden", includeHidden));

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        applications.Add(ApplicationModelFromReader(reader));
                    }
                }
            }

            return applications;
        }

        public int Create(ApplicationRegDTO dto)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
INSERT INTO T_APPLICATIONS
    (name, description, configuration_file)
VALUES
    (@name, @description, @configuration_file);
SELECT last_insert_rowid()";

                command.Parameters.Add(new SQLiteParameter("@name", dto.Name));
                command.Parameters.Add(new SQLiteParameter("@description", StringOrDBNull(dto.Description)));
                command.Parameters.Add(new SQLiteParameter("@configuration_file", dto.ConfigurationFile));
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int Update(ApplicationRegDTO dto)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
UPDATE T_APPLICATIONS
SET
     name = @name
    ,description = @description
    ,configuration_file = @configuration_file
WHERE id = @applicationId";

                command.Parameters.Add(new SQLiteParameter("@applicationId", dto.Id));
                command.Parameters.Add(new SQLiteParameter("@name", dto.Name));
                command.Parameters.Add(new SQLiteParameter("@description", StringOrDBNull(dto.Description)));
                command.Parameters.Add(new SQLiteParameter("@configuration_file", dto.ConfigurationFile));
                command.ExecuteNonQuery();

                return dto.Id;
            }
        }

        public void Delete(int applicationId)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
UPDATE T_APPLICATIONS
SET deleted = 1
WHERE id = @applicationId";

                command.Parameters.Add(new SQLiteParameter("@applicationId", applicationId));
                command.ExecuteNonQuery();
            }
        }

        public void ChangeVisibility(int applicationId, bool visible)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
UPDATE T_APPLICATIONS
SET visible = @visible
WHERE id = @applicationId";

                command.Parameters.Add(new SQLiteParameter("@applicationId", applicationId));
                command.Parameters.Add(new SQLiteParameter("@visible", visible));
                command.ExecuteNonQuery();
            }
        }

        private void CreateTables(SQLiteCommand command)
        {
            //Create T_APPLICATIONS table
            command.CommandText = @"
CREATE TABLE T_APPLICATIONS
(
    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    name TEXT(100) NOT NULL,
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