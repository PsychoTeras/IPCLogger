using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCLogger.ConfigurationService.DAL
{
    internal class ApplicationDAL : BaseDAL<ApplicationDAL>
    {
        private void CreateTables(SQLiteCommand command)
        {
            //Create T_APPLICATIONS table
            command.CommandText = @"
CREATE TABLE T_APPLICATIONS
(
    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    name TEXT(100) NOT NULL,
    description TEXT(160),
    log_file_path TEXT(260) NOT NULL,
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
