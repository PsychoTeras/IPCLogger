using IPCLogger.Loggers.Base;
using IPCLogger.Loggers.LDB.DAL;
using IPCLogger.Snippets;
using System;
using System.Collections.Generic;
using System.Data;

namespace IPCLogger.Loggers.LDB
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
            get { return _rows.Count >= Settings.QueueSize; }
        }

#endregion

#region Ctor

        public LDB(bool threadSafetyGuaranteed)
            : base(threadSafetyGuaranteed)
        {
        }

#endregion

#region ILogger

        protected override bool InitializeQueue()
        {
            _dal = new LoggerDAL(Settings.ConnectionString);
            Settings.InitializeTableSchema(_dal);
            InitializeDataTableStructure();
            return true;
        }

        protected override bool DeinitializeQueue()
        {
            if (_dataTable != null)
            {
                _dataTable.Dispose();
                _dataTable = null;
                return true;
            }
            return false;
        }

        protected override void WriteQueue(Type callerType, Enum eventType, string eventName,
            byte[] data, string text, bool writeLine)
        {
            DataRow row = _dataTable.NewRow();
            foreach (string columnName in Settings.Params)
            {
                object value;
                ColumnInfo ci = Settings.TableSchema[columnName];
                if (!Settings.Parameters.TryGetValue(columnName, out var pattern))
                {
                    value = DBNull.Value;
                }
                else
                {
                    value = SFactory.Process(callerType, eventType, data, text, pattern, Patterns);
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
