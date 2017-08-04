using System;

namespace IPCLogger.Core.Common
{
    internal static class Constants
    {
        public static readonly char Splitter = ',';
        public static readonly string ApplicableForAllMark = "*";
        public static readonly string NewLine = "\r\n";
        public static readonly int ProcessorsCount = Environment.ProcessorCount;
        public const string ROOT_LOGGER_CFG_PATH = "/configuration/IPCLogger";
    }
}
