using System;
using System.Collections.Generic;
using System.Data;
using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Loggers.LDB.DAL;
using IPCLogger.Core.Snippets;

namespace IPCLogger.Core.Loggers.LDB
{
    public sealed class LDB : QueueableLogger<LDBSettings>
    {

#region Private fields

        private LoggerDAL _dal;
        private DataTable _dataTable;
        private List<DataRow> _rows;

#endregion

#region Properties

        protected override bool ShouldFlushQueue
        {
            get { return _dataTable.Rows.Count >= Settings.QueueSize; }
        }

#endregion

#region ILogger

        protected override void InitializeQueue()
        {
            _dal = new LoggerDAL(Settings.ConnectionString);
            Settings.InitializeTableSchema(_dal);
            InitializeDataTableStructure();
        }

        protected override void DeinitializeQueue()
        {
            if (_dataTable != null)
            {
                _dataTable.Dispose();
                _dataTable = null;
            }
        }

        protected override void WriteQueue(Type callerType, Enum eventType, string eventName, 
            string text, bool writeLine)
        {
            DataRow row = _dataTable.NewRow();
            foreach (string columnName in Settings.Params)
            {
                object value;
                string pattern;
                ColumnInfo ci = Settings.TableSchema[columnName];
                if (!Settings.ParamValues.TryGetValue(columnName, out pattern))
                {
                    value = DBNull.Value;
                }
                else
                {
                    value = SFactory.Process(callerType, eventType, text, pattern, Patterns);
                    value = value == null ? DBNull.Value : Convert.ChangeType(value, ci.Type);
                }
                row[columnName] = value;
            }
            _rows.Add(row);
        }

        protected override void FlushQueue()
        {
            if (_rows.Count > 0)
            {
                _dal.WriteLog(_dataTable.TableName, _rows.ToArray());
                _rows.Clear();
            }
        }

#endregion

#region Class methods

        private void InitializeDataTableStructure()
        {
            _dataTable = new DataTable(Settings.TableName);
            foreach (ColumnInfo columnInfo in Settings.TableSchema.Values)
            {
                _dataTable.Columns.Add(columnInfo.Name, columnInfo.Type);
            }
            _rows = new List<DataRow>(Settings.QueueSize);
        }

#endregion

    }
}
