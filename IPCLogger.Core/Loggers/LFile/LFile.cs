using System;
using System.IO;
using System.Text;
using IPCLogger.Core.Common;
using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Snippets;

namespace IPCLogger.Core.Loggers.LFile
{
    public sealed class LFile : ConcurrentLogger<LFileSettings>
    {

#region Private fields

        private static readonly Encoding _utf8 = new UTF8Encoding(false);

        private string _fileName;
        private Stream _fileStream;
        private long _fileStreamSize;
        private StreamWriter _logWriter;

        private int _lastTimeMark;
        private DateTime _lastFilePathCheckTime;
        private int _lastFilePathCheckHour;

        private int _logCurrentIdx;
        private DateTime? _logRollingDateTime;
        private string _logGenericPath;
        private bool _logGenericPathIsConstant;

#endregion

#region Ctor

        public LFile(bool threadSafetyIsGuaranteed) 
            : base(threadSafetyIsGuaranteed)
        {
        }

#endregion

#region ILogger

        protected override void WriteConcurrent(Type callerType, Enum eventType, string eventName,
            byte[] data, string text, bool writeLine)
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
            _lastFilePathCheckTime = DateTime.Now;
            _lastFilePathCheckHour = _lastFilePathCheckTime.Hour;
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
            //Roll By The File Age check / Roll By The File Path check
            bool rollByFileAge = false, rollByFilePath = false;
            if (Settings.RollByFileAge && _logRollingDateTime.HasValue || Settings.DynamicFilePath)
            {
                int ticks = Environment.TickCount;
                int currentTimeMark = ticks - ticks%1000; //Each 1 second
                if (currentTimeMark != _lastTimeMark)
                {
                    //Check RollByFileAge
                    if (Settings.RollByFileAge && _logRollingDateTime.HasValue)
                    {
                        rollByFileAge = DateTime.UtcNow >= _logRollingDateTime.Value;
                    }

                    //Check DynamicFilePath
                    if (Settings.DynamicFilePath)
                    {
                        DateTime localNow = DateTime.Now;
                        int localHour = localNow.Hour;
                        rollByFilePath = localHour != _lastFilePathCheckHour;
                        if (rollByFilePath)
                        {
                            _lastFilePathCheckTime = localNow;
                            _lastFilePathCheckHour = localHour;
                        }
                    }

                    _lastTimeMark = currentTimeMark;
                }
            }

            //Roll By The File Size check
            bool rollByFileSize = Settings.RollByFileSize && _fileStreamSize >= Settings.MaxFileSize;

            //Show we do rolling the log at all
            bool shouldRollTheLog = rollByFileAge || rollByFileSize || rollByFilePath;

            if (_fileStream == null || shouldRollTheLog)
            {
                string logGenericPath;
                if (_fileStream == null && !unsuspend)
                {
                    logGenericPath = SFactory.Process(Settings.ExpandedLogFilePathWithMark, Patterns);

                    //Is the file path is not dynamic turn off the appropriate processing snippet
                    if (Settings.ExpandedLogFilePathWithMark == logGenericPath)
                    {
                        Settings.DynamicFilePath = false;
                        _logGenericPathIsConstant = true;
                    }

                    _logGenericPath = logGenericPath;
                }

                logGenericPath = _logGenericPathIsConstant
                    ? _logGenericPath
                    : SFactory.Process(Settings.ExpandedLogFilePathWithMark, Patterns);

                //If the log path has been changed then reset current postfix (_logCurrentIdx) and store the generic path value
                //otherwise just increment postfix to supply rolling
                bool logPathHasBeenChanged = false;
                if (shouldRollTheLog)
                {
                    logPathHasBeenChanged = !_logGenericPathIsConstant && logGenericPath != _logGenericPath;
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

                //Exit if the log path has not been changed
                if (rollByFilePath && !rollByFileAge && !rollByFileSize && !logPathHasBeenChanged)
                {
                    return;
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
                        logPath = logGenericPath.Replace(LFileSettings.IdxPlaceMark,
                            _logCurrentIdx == 0 ? string.Empty : "_" + _logCurrentIdx);

                        bool logFileExists = (shouldRollTheLog || !Settings.RecreateFile) &&
                                             Helpers.PathFileExists(logPath);
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

                _fileStream = Settings.BufferSize <= 0
                    ? new FileStream(_fileName = logPath, fileMode, FileAccess.Write, FileShare.Read)
                    : new FileStream(_fileName = logPath, fileMode, FileAccess.Write, FileShare.Read, Settings.BufferSize);

                _fileStreamSize = fileMode == FileMode.Append ? _fileStream.Length : 0;

                _logWriter = Settings.BufferSize <= 0
                    ? new StreamWriter(_fileStream, _utf8) {AutoFlush = true}
                    : new StreamWriter(_fileStream, _utf8, Settings.BufferSize);
            }
        }

#endregion

    }
}