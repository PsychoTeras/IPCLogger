using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using IPCLogger.Core.Common;
using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Loggers.LFactory;
using IPCLogger.Core.Storages;
using IPCLogger.TestService.Common;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;

namespace IPCLogger.TestService
{
    static class Program
    {
        //--------------------------------------------Configure test environment here----------------------------------------------

        private static readonly WaitCallback _workMethod = WriteLogIPC;                 //Do we use WriteLogIPC or WriteLog4Net?
        private static readonly int _parallelOperations = Environment.ProcessorCount;   //Number of parallel operations (Environment.ProcessorCount)
        private static readonly int _recordsCount = 500000 / _parallelOperations;       //Number of iterations

        //-------------------------------------------------------------------------------------------------------------------------

        static WaitHandle[] _tEvents;
        private static HRTimer _timer;
        private static Guid _guid = new Guid();
        private static string _sGuid = _guid.ToString();
        private static ILog _logger = LogManager.GetLogger(typeof(Program));

        internal static void WriteLog4Net(object obj)
        {
            _logger.Info(_sGuid); //'Cold' write

            _timer = HRTimer.CreateAndStart();
            for (int i = 0; i < _recordsCount - 1; i++)
            {
                _logger.Info(_sGuid);
            }

            ((ManualResetEvent) obj).Set();
        }

        internal static void WriteLogIPC(object obj)
        {
            using (TLSObject tlsObj = TLS.Push())
            {
                tlsObj["_int"] = 0xACDC;
                tlsObj["_nvarchar"] = _sGuid;
                tlsObj["_uniqueidentifier"] = _guid;

                //ApplicableForTester.WriteMessage();
                LFactory.Instance.Write(LogEvent.Info, _sGuid); //'Cold' write

                _timer = HRTimer.CreateAndStart();
                for (int i = 0; i < _recordsCount - 1; i++)
                {
                    //Thread.Sleep(1000);
                    LFactory.Instance.Write(LogEvent.Info, _sGuid);
                }
            }

            ((ManualResetEvent)obj).Set();
        }

        private static void FlushLog4NetBuffers()
        {
            ILoggerRepository rep = LogManager.GetRepository();
            foreach (IAppender appender in rep.GetAppenders())
            {
                var buffered = appender as BufferingAppenderSkeleton;
                if (buffered != null)
                {
                    buffered.Flush();
                }
            }
        }

        private static void Echo(string[] args)
        {
            if (_workMethod == WriteLogIPC)
            {
                LFactory.Instance.Initialize();
            }
            else
            {
                XmlConfigurator.Configure(new FileInfo(@"IPCLogger.TestService.exe.config"));
            }

            _tEvents = new WaitHandle[_parallelOperations];
            for (int i = 0; i < _parallelOperations; i++)
            {
                _tEvents[i] = new ManualResetEvent(false);
            }

            for (int i = 0; i < _parallelOperations; i++)
            {
                ThreadPool.QueueUserWorkItem(_workMethod, _tEvents[i]);
            }

            WaitHandle.WaitAll(_tEvents);

            if (_workMethod == WriteLogIPC)
            {
                LFactory.Instance.Flush();
            }
            else
            {
                FlushLog4NetBuffers();
            }
        }
        
        static void Main(string[] param)
        {
            //_timer = HRTimer.CreateAndStart();

            //Console.WriteLine(_timer.StopWatch());
            //Console.ReadKey();
            //Process.GetCurrentProcess().Kill();
            //return;

            LFactory.LoggerException += LoggerException;
            Thread.CurrentThread.Name = "MainThread";
            if (ServiceHelper.IsBeingRunAsService())
            {
                ServiceBase[] servicesToRun =
                {
                    new CommonService(Echo)
                };
                ServiceBase.Run(servicesToRun);
            }
            else
            {
                Echo(param);
                Console.WriteLine(_timer.StopWatch());
                Console.ReadKey();
                Process.GetCurrentProcess().Kill();
            }
        }

        static void LoggerException(IBaseLogger sender, Exception ex)
        {
            string stackTrace = string.Empty;
            StringBuilder sb = new StringBuilder("Logger exception: ");
            do
            {
                if (string.IsNullOrEmpty(ex.Message)) continue;
                sb.AppendLine(ex.Message);
                if (stackTrace == string.Empty && ex.StackTrace != null)
                {
                    stackTrace = ex.StackTrace;
                }
            } while ((ex = ex.InnerException) != null);
            sb.AppendLine(stackTrace);
            Console.WriteLine(sb.ToString());
        }
    }
}
