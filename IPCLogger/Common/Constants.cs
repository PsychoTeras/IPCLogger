﻿using System;

namespace IPCLogger.Common
{
    internal static class Constants
    {
        public static readonly char Splitter = ',';
        public static readonly string SplitterString = Splitter + " ";
        public static readonly string ApplicableForAllMark = "*";
        public static readonly string NewLine = "\r\n";
        public static readonly int ProcessorsCount = Environment.ProcessorCount;
        public static readonly bool Is64Bit = IntPtr.Size == 8;
        public const string LoggerName = "IPCLogger";
        public const string RootAppCfgPath = "/configuration/" + LoggerName;
        public const string RootPatternsCfgPath = RootAppCfgPath + "/Patterns";
        public const string RootLoggersCfgPath = RootAppCfgPath + "/Loggers";
    }
}
