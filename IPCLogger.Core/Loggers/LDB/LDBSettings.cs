using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Loggers.LDB.DAL;

namespace IPCLogger.Core.Loggers.LDB
{
    public sealed class LDBSettings : QueueableSettings
    {

#region Constants

        private const string PARAMS_NODE_NAME = "Parameters";

#endregion

#region Internal fields

        internal Dictionary<string, ColumnInfo> TableSchema;
        internal string[] Params;
        internal Dictionary<string, string> ParamValues;

#endregion

#region Properties

        public string ConnectionString { get; set; }

        public string TableName { get; set; }

#endregion

#region Ctor

        public LDBSettings(Type loggerType, Action onApplyChanges) 
            : base(loggerType, onApplyChanges) { }

#endregion

#region Class methods

        private void ReadDbParameters(XmlNode cfgNode)
        {
            ParamValues = new Dictionary<string, string>();

            XmlNode paramsNode = cfgNode.SelectSingleNode(PARAMS_NODE_NAME);
            if (paramsNode == null) return;

            XmlNodeList paramNodes = paramsNode.ChildNodes;
            foreach (XmlNode paramNode in paramNodes)
            {
                if (ParamValues.ContainsKey(paramNode.Name))
                {
                    string msg = $"Duplicated DB parameter definition '{paramNode.Name}'";
                    throw new Exception(msg);
                }
                ParamValues.Add(paramNode.Name, paramNode.InnerText);
            }
        }

        protected override Dictionary<string, string> GetSettingsDictionary(XmlNode cfgNode)
        {
            ReadDbParameters(cfgNode);
            return GetSettingsDictionary(cfgNode, new[] { PARAMS_NODE_NAME });
        }

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
