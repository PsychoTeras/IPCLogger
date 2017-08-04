using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using IPCLogger.Core.Common;
using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Loggers.LFactory;
using IPCLogger.TestService.Common;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;

namespace IPCLogger.TestService
{
    internal static class Program
    {
        //--------------------------------------------Configure test environment here----------------------------------------------

        private static readonly WaitCallback WorkMethod = WriteLogIPC;                  //Do we use WriteLogIPC or WriteLog4Net?
        private static readonly int ParallelOperations = Environment.ProcessorCount;    //Number of parallel operations (Environment.ProcessorCount)
        private static readonly int RecordsCount = 500000 / ParallelOperations;         //Number of iterations

        //-------------------------------------------------------------------------------------------------------------------------

        static WaitHandle[] _tEvents;
        private static HRTimer _timer;
        private static Guid _guid = new Guid();
        private static string _sGuid = _guid.ToString();
        private static ILog _logger = LogManager.GetLogger(typeof(Program));

        //-------------------------------------------------------------------------------------------------------------------------

        enum CtrlType
        {
            CtrlCEvent = 0,
            CtrlCloseEvent = 2,
            CtrlLogoffEvent = 5,
            CtrlShutdownEvent = 6
        }

        [DllImport("kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        private static EventHandler _handler;

        private static bool ConsoleCloseHandler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CtrlCEvent:
                case CtrlType.CtrlLogoffEvent:
                case CtrlType.CtrlShutdownEvent:
                case CtrlType.CtrlCloseEvent:
                {
                    if (WorkMethod == null) return false;
                    if (WorkMethod == WriteLogIPC)
                    {
                        LFactory.Instance.Flush();
                    }
                    else
                    {
                        FlushLog4NetBuffers();
                    }
                    return false;
                }
            }
            return true;
        }

        //-------------------------------------------------------------------------------------------------------------------------

        internal static void WriteLog4Net(object obj)
        {
            _logger.Info(_sGuid); //'Cold' write

            _timer = HRTimer.CreateAndStart();
            for (int i = 0; i < RecordsCount - 1; i++)
            {
                _logger.Info(_sGuid);
            }

            ((ManualResetEvent) obj).Set();
        }

        internal static int GetInt()
        {
            return -1;
        }

        internal static void WriteLogIPC(object obj)
        {
            LFactory.Instance.Write(LogEvent.Info, _sGuid); //'Cold' write

            _timer = HRTimer.CreateAndStart();
            for (int i = 0; i < RecordsCount - 1; i++)
            {
                Thread.Sleep(1);
                LFactory.Instance.Write(LogEvent.Info, _sGuid);
            }

            ((ManualResetEvent) obj).Set();
        }

        private static void FlushLog4NetBuffers()
        {
            ILoggerRepository rep = LogManager.GetRepository();
            foreach (IAppender appender in rep.GetAppenders())
            {
                (appender as BufferingAppenderSkeleton)?.Flush();
            }
        }

        private static void Echo(string[] args)
        {
            if (WorkMethod == WriteLogIPC)
            {
                LFactory.Instance.Initialize();
            }
            else
            {
                XmlConfigurator.Configure(new FileInfo(@"IPCLogger.TestService.exe.config"));
            }

            _tEvents = new WaitHandle[ParallelOperations];
            for (int i = 0; i < ParallelOperations; i++)
            {
                _tEvents[i] = new ManualResetEvent(false);
            }

            for (int i = 0; i < ParallelOperations; i++)
            {
                ThreadPool.QueueUserWorkItem(WorkMethod, _tEvents[i]);
            }

            WaitHandle.WaitAll(_tEvents);

            if (WorkMethod == WriteLogIPC)
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
            //LFactory.Instance.Write(LogEvent.Debug, (string)null);

            //string value = "data";
            //using (TLSObject tlsObj = TLS.Push())
            //{
            //    tlsObj.SetClosure(() => value);

            //    _timer = HRTimer.CreateAndStart();
            //    for (int i = 0; i < _recordsCount - 1; i++)
            //    {
            //        LFactory.Instance.WriteLine(LogEvent.Debug, (string)null);
            //    }
            //}

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
                _handler += ConsoleCloseHandler;
                SetConsoleCtrlHandler(_handler, true);
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
