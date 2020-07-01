using IPCLogger.Attributes;
using IPCLogger.Attributes.CustomConversionAttributes;
using IPCLogger.Loggers.Base;
using IPCLogger.Loggers.LDB.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IPCLogger.Loggers.LDB
{
    public sealed class LDBSettings : QueueableSettings
    {

#region Internal fields

        internal Dictionary<string, ColumnInfo> TableSchema;
        internal string[] Params;

#endregion

#region Properties

        [RequiredSetting]
        public string ConnectionString { get; set; }

        [RequiredSetting]
        public string TableName { get; set; }

        [KeyValueConversion(typeof(Dictionary<string, string>), "Column name"), RequiredSetting, FormattableSetting]
        public Dictionary<string, string> Parameters { get; set; }

#endregion

#region Ctor

        public LDBSettings(Type loggerType, Action onApplyChanges) 
            : base(loggerType, onApplyChanges) { }

#endregion

#region Class methods

        internal void InitializeTableSchema(LoggerDAL dal)
        {
            List<ColumnInfo> columns = dal.GetTableColumns(TableName);
            TableSchema = new Dictionary<string, ColumnInfo>(columns.Count);
            foreach (ColumnInfo column in columns)
            {
                TableSchema.Add(column.Name, column);
            }
            Params = columns.Where(c => !c.IsIdentity).Select(c => c.Name).ToArray();
        }

#endregion

    }
}
