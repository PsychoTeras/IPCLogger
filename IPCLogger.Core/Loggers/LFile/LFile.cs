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

        private string _fileName;
        private Stream _fileStream;
        private long _fileStreamSize;
        private StreamWriter _logWriter;

        private int _lastTimeMark;

        private int _logCurrentIdx;
        private DateTime? _logRollingDateTime;
        private string _logGenericPath;

#endregion

#region Ctor

        public LFile(bool threadSafetyIsGuaranteed) 
            : base(threadSafetyIsGuaranteed)
        {
        }

#endregion

#region ILogger

        protected override void WriteConcurrent(Type callerType, Enum eventType, string eventName, 
            string text, bool writeLine)
        {
            if (writeLine) text += Constants.NewLine;
            PrepareLogFileStream(false);
            _logWriter.Write(text);
            _fileStreamSize += text.Length;
        }

        protected override bool InitializeConcurrent()
        {
            _logCurrentIdx = 0;
            _fileStreamSize = 0;
            _fileName = null;
            _logGenericPath = null;
            _logRollingDateTime = null;
            PrepareLogFileStream(false);
            return true;
        }

        protected override bool DeinitializeConcurrent()
        {
            Flush();
            DestroyLogFileStream(false);
            return true;
        }

        protected override void FlushConcurrent()
        {
            _logWriter.Flush();
        }

        protected override bool SuspendConcurrent()
        {
            Flush();
            DestroyLogFileStream(true);
            return true;
        }

        protected override bool ResumeConcurrent()
        {
            PrepareLogFileStream(true);
            return true;
        }

#endregion

#region Class methods
        
        private void DestroyLogFileStream(bool suspend)
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
                    if (!suspend)
                    {
                        _fileName = null;
                        _fileStreamSize = 0;
                    }
                }
            }
        }

        private void PrepareLogFileStream(bool unsuspend)
        {
            //Roll By The File Age check
            bool rollByFileAge = false;
            if (Settings.RollByFileAge && _logRollingDateTime.HasValue)
            {
                int ticks = Environment.TickCount;
                int currentTimeMark = ticks - ticks%1000;
                if (currentTimeMark != _lastTimeMark)
                {
                    rollByFileAge = _logRollingDateTime.Value <= DateTime.UtcNow;
                    _lastTimeMark = currentTimeMark;
                }
            }

            //Roll By The File Size check
            bool rollByFileSize = Settings.RollByFileSize && _fileStreamSize >= Settings.MaxFileSize;

            bool shouldRollTheLog = rollByFileAge || rollByFileSize;

            if (_fileStream == null || shouldRollTheLog)
            {
                //If the log path has been changed then reset current postfix (_logCurrentIdx)
                //otherwise just increment postfix to supply rolling
                if (shouldRollTheLog || _logGenericPath == null)
                {
                    string logGenericPath = Path.Combine(Settings.LogDir, Settings.LogFile);
                    logGenericPath = SFactory.Process(logGenericPath, Patterns);

                    bool logPathHasBeenChanged = _logGenericPath == null || logGenericPath != _logGenericPath;
                    if (logPathHasBeenChanged)
                    {
                        _logCurrentIdx = 0;
                        _logGenericPath = logGenericPath;
                    }
                    else
                    {
                        _logCurrentIdx++;
                    }
                }

                DestroyLogFileStream(false);

                string logPath;
                if (unsuspend && !shouldRollTheLog)
                {
                    logPath = _fileName;
                    if (Settings.RollByFileAge && !Helpers.PathFileExists(logPath))
                    {
                        _logRollingDateTime = DateTime.UtcNow.Add(Settings.MaxFileAge);
                    }
                }
                else
                {
                    //Get log file path and save the data for the Roll By The File Age check
                    do
                    {
                        string logFile = _logCurrentIdx == 0
                            ? Settings.LogFile
                            : string.Format("{0}_{1}{2}", Settings.LogFileName, _logCurrentIdx, Settings.LogFileExt);
                        logPath = Path.Combine(Settings.LogDir, logFile);
                        logPath = Environment.ExpandEnvironmentVariables(logPath);
                        logPath = SFactory.Process(logPath, Patterns);

                        bool logFileExists = (shouldRollTheLog || !Settings.RecreateFile) && Helpers.PathFileExists(logPath);
                        if (shouldRollTheLog && logFileExists && !Settings.RecreateFile)
                        {
                            _logCurrentIdx++;
                            logPath = null;
                            continue;
                        }

                        if (Settings.RollByFileAge)
                        {
                            DateTime logCurrentDateTime = logFileExists
                                ? File.GetCreationTimeUtc(logPath)
                                : DateTime.UtcNow;

                            //Role if !Settings.RecreateFile and file is obsolote by creation date
                            if (logFileExists && DateTime.UtcNow.Subtract(logCurrentDateTime) >= Settings.MaxFileAge)
                            {
                                shouldRollTheLog = true;
                                _logCurrentIdx++;
                                logPath = null;
                                continue;
                            }

                            _logRollingDateTime = logCurrentDateTime.Add(Settings.MaxFileAge);
                        }

                    } while (logPath == null);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(logPath));

                FileMode fileMode;
                if (Settings.RecreateFile && (!unsuspend || shouldRollTheLog))
                {
                    fileMode = FileMode.Create;
                }
                else
                {
                    fileMode = FileMode.Append;
                }

                _fileStream = Settings.BufferSize > 0
                    ? new FileStream(_fileName = logPath, fileMode, FileAccess.Write, FileShare.Read, Settings.BufferSize)
                    : new FileStream(_fileName = logPath, fileMode, FileAccess.Write, FileShare.Read);

                _fileStreamSize = fileMode == FileMode.Append ? _fileStream.Length : 0;

                _logWriter = new StreamWriter(_fileStream)
                {
                    AutoFlush = Settings.BufferSize == 0
                };
            }
        }

#endregion

    }
}
