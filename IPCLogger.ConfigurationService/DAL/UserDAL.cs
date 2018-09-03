using IPCLogger.ConfigurationService.Entities;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace IPCLogger.ConfigurationService.DAL
{
    internal class UserDAL : BaseDAL<UserDAL>, IUserMapper
    {
        class DefaultUserRole
        {
            public string UserName;

            public string Description;

            public List<int> ClaimsIds;
        }

        class DefaultUser : UserAuthDTO
        {
            public int RoleId { get; set; }
        }

        private readonly List<KeyValuePair<string, string>> _claims = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("ViewLogSettings", "View log settings"),
            new KeyValuePair<string, string>("ModifyLogSettings", "Modify log settings"),
            new KeyValuePair<string, string>("ViewLogs", "View logs"),
            new KeyValuePair<string, string>("RequestLogs", "Request logs")
        };

        private readonly List<DefaultUserRole> _defaultRoles = new List<DefaultUserRole>
        {
            new DefaultUserRole
            {
                UserName = "Administrator",
                Description = "Administrator",
                ClaimsIds = new List<int> {1, 2, 3, 4}
            },
            new DefaultUserRole
            {
                UserName = "User",
                Description = "User",
                ClaimsIds = new List<int> {1, 3, 4}
            },
        };

        private readonly List<DefaultUser> _defaultUsers = new List<DefaultUser>
        {
            new DefaultUser
            {
                UserName = "admin",
                Password = "admin",
                RoleId = 1
            }
        };

        public IUserIdentity GetUserFromIdentifier(Guid guid, NancyContext context)
        {
            int userId;
            User user = new User
            {
                Claims = new List<string>()
            };

            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT
     id
    ,user_name
FROM T_USER
WHERE guid = @guid";

                command.Parameters.Add(new SQLiteParameter("@guid", guid.ToString()));

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new Exception("Wrong session");
                    }
                    userId = Convert.ToInt32(reader["id"]);
                    user.UserName = reader["user_name"].ToString();
                }

                command.CommandText = @"
SELECT c.name
FROM T_USER_ROLE ur
JOIN T_ROLES r ON
    r.id = ur.role_id
JOIN T_ROLE_CLAIMS rc ON
    rc.role_id = r.id
JOIN T_CLAIMS c ON
    c.id = rc.claim_id
WHERE ur.user_id = @user_id";

                command.Parameters.Clear();
                command.Parameters.Add(new SQLiteParameter("@user_id", userId));

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ((List<string>)user.Claims).Add(reader["name"].ToString());
                    }
                }
            }

            return user;
        }

        public Guid? Login(UserAuthDTO dto)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT guid
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

        public void Register(UserRegDTO dto)
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

        public List<Claim> GetClaims()
        {
            List<Claim> claims = new List<Claim>();

            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT *
FROM T_CLAIMS";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        claims.Add(new Claim
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString()
                        });
                    }
                }
            }

            return claims;
        }

        public List<Role> GetRoles()
        {
            List<Role> claims = new List<Role>();

            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT *
FROM T_ROLES";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        claims.Add(new Role
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString()
                        });
                    }
                }
            }

            return claims;
        }

        private void CreateTables(SQLiteCommand command)
        {
            //Create T_ROLES table
            command.CommandText = @"
CREATE TABLE T_ROLES
(
    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    name TEXT(39) NOT NULL UNIQUE COLLATE NOCASE,
    description TEXT(160)
)";
            command.ExecuteNonQuery();

            //Create T_CLAIMS table
            command.CommandText = @"
CREATE TABLE T_CLAIMS
(
    id INTEGER PRIMARY KEY NOT NULL,
    name TEXT(39) NOT NULL UNIQUE COLLATE NOCASE,
    description TEXT(160)
)";
            command.ExecuteNonQuery();

            //Create T_ROLE_CLAIMS table
            command.CommandText = @"
CREATE TABLE T_ROLE_CLAIMS
(
    role_id INTEGER NOT NULL,
    claim_id INTEGER NOT NULL,
    FOREIGN KEY(role_id) REFERENCES T_ROLES(id),
    FOREIGN KEY(claim_id) REFERENCES T_CLAIMS(id)
)";
            command.ExecuteNonQuery();

            //Create T_USER table
            command.CommandText = @"
CREATE TABLE T_USER
(
    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    user_name TEXT(39) NOT NULL UNIQUE COLLATE NOCASE,
    password_hash TEXT(32) NOT NULL,
    guid TEXT(36) NOT NULL,
    blocked BOOLEAN DEFAULT 0 NOT NULL    
)";
            command.ExecuteNonQuery();

            //Create IDX_T_USER_AUTH index
            command.CommandText = @"
CREATE UNIQUE INDEX IDX_T_USER_AUTH
ON [T_USER] (user_name, password_hash)
";
            command.ExecuteNonQuery();

            //Create T_USER_ROLE table
            command.CommandText = @"
CREATE TABLE T_USER_ROLE
(
    user_id INTEGER NOT NULL,
    role_id INTEGER NOT NULL,
    FOREIGN KEY(user_id) REFERENCES T_USERS(id),
    FOREIGN KEY(role_id) REFERENCES T_ROLES(id)
)";
            command.ExecuteNonQuery();
        }

        private void CreateTablesData(SQLiteCommand command)
        {
            //Create claims
            command.CommandText = @"
INSERT INTO T_CLAIMS
    (id, name, description)
VALUES
    (@id, @name, @description)";

            for (int i = 0; i < _claims.Count; i++)
            {
                KeyValuePair<string, string> claim = _claims[i];
                command.Parameters.Clear();
                command.Parameters.Add(new SQLiteParameter("@id", i + 1));
                command.Parameters.Add(new SQLiteParameter("@name", claim.Key));
                command.Parameters.Add(new SQLiteParameter("@description", claim.Value));
                command.ExecuteNonQuery();
            }

            //Create default user roles
            for (int i = 0; i < _defaultRoles.Count; i++)
            {
                command.CommandText = @"
INSERT INTO T_ROLES
    (name, description)
VALUES
    (@name, @description);
SELECT last_insert_rowid()";

                DefaultUserRole defaultRole = _defaultRoles[i];
                command.Parameters.Clear();
                command.Parameters.Add(new SQLiteParameter("@name", defaultRole.UserName));
                command.Parameters.Add(new SQLiteParameter("@description", defaultRole.Description));
                int roleId = Convert.ToInt32(command.ExecuteScalar());

                command.CommandText = @"
INSERT INTO T_ROLE_CLAIMS
    (role_id, claim_id)
VALUES
    (@role_id, @claim_id)";

                foreach (int claimId in defaultRole.ClaimsIds)
                {
                    command.Parameters.Clear();
                    command.Parameters.Add(new SQLiteParameter("@role_id", roleId));
                    command.Parameters.Add(new SQLiteParameter("@claim_id", claimId));
                    command.ExecuteNonQuery();
                }
            }

            //Create default users
            command.CommandText = @"
INSERT INTO T_USER
    (user_name, password_hash, guid)
VALUES
    (@user_name, @password_hash, @guid);
SELECT last_insert_rowid()";

            foreach (DefaultUser defaultUser in _defaultUsers)
            {
                command.Parameters.Clear();
                command.Parameters.Add(new SQLiteParameter("@user_name", defaultUser.UserName));
                command.Parameters.Add(new SQLiteParameter("@password_hash", defaultUser.PasswordHash));
                command.Parameters.Add(new SQLiteParameter("@guid", Guid.NewGuid().ToString()));
                int userId = Convert.ToInt32(command.ExecuteScalar());

                command.CommandText = @"
INSERT INTO T_USER_ROLE
    (user_id, role_id)
VALUES
    (@user_id, @role_id)";

                command.Parameters.Clear();
                command.Parameters.Add(new SQLiteParameter("@user_id", userId));
                command.Parameters.Add(new SQLiteParameter("@role_id", defaultUser.RoleId));
                command.ExecuteNonQuery();
            }
        }

        protected override void CreateDbStructure()
        {
            using (SQLiteTransaction transaction = Connection.BeginTransaction())
            {
                using (SQLiteCommand command = new SQLiteCommand(Connection))
                {
                    //Create tables structure
                    CreateTables(command);

                    //Cerate tables data
                    CreateTablesData(command);

                    //Commit
                    transaction.Commit();
                }
            }
        }
    }
}