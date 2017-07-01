using System;
using System.IO;
using IPCLogger.Core.Common;
using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Snippets;

namespace IPCLogger.Core.Loggers.LFile
{
    public sealed class LFile : ConcurrentLogger<LFileSettings>
    {

#region Private fields

        private string _logFileName;
        private Stream _fileStream;
        private StreamWriter _logWriter;

        private DateTime _currentDay;
        private static int _lastDateMark;
        private DateTime _lastDate;

#endregion

#region ILogger

        protected override void WriteConcurrent(Type callerType, Enum eventType, string eventName, 
            string text, bool writeLine)
        {
            if (writeLine) text += Constants.NewLine;
            PrepareLogFileStream(false);
            _logWriter.Write(text);
        }

        protected override void InitializeConcurrent()
        {
            _currentDay = DateTime.Now;
            PrepareLogFileStream(false);
        }

        protected override void DeinitializeConcurrent()
        {
            Flush();
            DestroyLogFileStream();
        }

        protected override void FlushConcurrent()
        {
            _logWriter.Flush();
        }

#endregion

#region Class methods
        
        private void DestroyLogFileStream()
        {
            if (_fileStream != null)
            {
                _logWriter.Flush();
                try
                {
                    _logWriter.Dispose();
                    _fileStream.Dispose();
                }
                catch { }
                finally
                {
                    _logWriter = null;
                    _fileStream = null;
                }
            }
        }

        private void PrepareLogFileStream(bool unsuspend)
        {
            int ticks = Environment.TickCount;
            int currentTimeMark = ticks - ticks%1000;
            if (currentTimeMark != _lastDateMark)
            {
                _lastDate = DateTime.Now;
                _lastDateMark = currentTimeMark;
            }

            bool nextDay = (int)_lastDate.Subtract(_currentDay).TotalDays != 0;
            if (nextDay || _fileStream == null)
            {
                if (nextDay)
                {
                    _currentDay = _lastDate;
                }

                DestroyLogFileStream();

                Directory.CreateDirectory(Settings.LogDir);

                string logFile = SFactory.Process(Settings.LogFile, Patterns);
                string logPath = Path.Combine(Settings.LogDir, logFile);
                if ((nextDay && !logPath.Equals(_logFileName, StringComparison.InvariantCultureIgnoreCase) || 
                    (!nextDay && !unsuspend && Settings.RecreateFile)) && 
                    File.Exists(logPath))
                {
                    File.Delete(logPath);
                }

                if (Settings.QueueSize > 0)
                {
                    _fileStream = new FileStream(_logFileName = logPath, FileMode.Append, FileAccess.Write, 
                        FileShare.Read, Settings.QueueSize);
                }
                else
                {
                    _fileStream = new FileStream(_logFileName = logPath, FileMode.Append, FileAccess.Write,
                        FileShare.Read);
                }
                _logWriter = new StreamWriter(_fileStream)
                {
                    AutoFlush = Settings.QueueSize == 0
                };
            }
        }

#endregion

    }
}
