using IPCLogger.ConfigurationService.Entities;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using System;
using System.Data.SQLite;

namespace IPCLogger.ConfigurationService.DAL
{
    internal class UserDAL : BaseDAL<UserDAL>, IUserMapper
    {
        public IUserIdentity GetUserFromIdentifier(Guid guid, NancyContext context)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT
    *
FROM T_USER
WHERE
    guid = @guid";

                command.Parameters.Add(new SQLiteParameter("@guid", guid.ToString()));

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new Exception("Wrong session");
                    }
                    return new User
                    {
                        UserName = reader["user_name"].ToString()
                    };
                }
            }
        }

        public Guid? Login(UserAuthDTO dto)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT
    guid
FROM T_USER
WHERE
    user_name = @user_name AND
    password_hash = @password_hash";

                command.Parameters.Add(new SQLiteParameter("@user_name", dto.UserName));
                command.Parameters.Add(new SQLiteParameter("@password_hash", dto.PasswordHash));

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    return reader.Read() ? new Guid(reader["guid"].ToString()) : (Guid?) null;
                }
            }
        }

        public void Register(UserAuthDTO dto)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
INSERT OR IGNORE INTO T_USER
    (user_name, password_hash, guid)
VALUES
    (@user_name, @password_hash, @guid)";

                command.Parameters.Add(new SQLiteParameter("@user_name", dto.UserName));
                command.Parameters.Add(new SQLiteParameter("@password_hash", dto.PasswordHash));
                command.Parameters.Add(new SQLiteParameter("@guid", Guid.NewGuid().ToString()));

                int result = command.ExecuteNonQuery();
            }
        }

        protected override void CreateDbStructure()
        {
            using (SQLiteTransaction transaction = Connection.BeginTransaction())
            {
                using (SQLiteCommand command = new SQLiteCommand(Connection))
                {
                    //Create TABLE table
                    command.CommandText = @"
CREATE TABLE T_USER
(
    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    user_name TEXT(39) NOT NULL UNIQUE COLLATE NOCASE,
    password_hash TEXT(32) NOT NULL,
    guid TEXT(36) NOT NULL,
    is_active BOOLEAN DEFAULT 1 NOT NULL    
)";
                    command.ExecuteNonQuery();

                    //Create IDX_T_USER_AUTH index
                    command.CommandText = @"
CREATE UNIQUE INDEX IDX_T_USER_AUTH
ON [T_USER] (user_name, password_hash)
";
                    command.ExecuteNonQuery();

                    //Commit
                    transaction.Commit();
                }
            }
        }
    }
}
