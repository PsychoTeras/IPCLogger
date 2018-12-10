using IPCLogger.Core.Common;
using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Loggers.LFactory;
using IPCLogger.Core.Storages;
using IPCLogger.TestService.Common;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace IPCLogger.TestService
{
    internal static class Program
    {
        //--------------------------------------------Configure test environment here----------------------------------------------

        private static readonly WaitCallback _workMethod = WriteLogIPC;                  //Do we use WriteLogIPC or WriteLog4Net?
        private static readonly bool IsIPCLogger = _workMethod == WriteLogIPC;           //Whether target logger is IPCLogger
        private static readonly int _parallelOperations = Environment.ProcessorCount;    //Number of parallel operations (Environment.ProcessorCount)
        private static readonly int _recordsCount = 5000000 / _parallelOperations;       //Number of iterations

        //-------------------------------------------------------------------------------------------------------------------------

        private static HRTimer _timer;
        private static WaitHandle[] _tEvents;
        private static Guid _guid = new Guid();
        private static string _sGuid = _guid.ToString();
        private static ILog _logger = LogManager.GetLogger(typeof(Program));

        private static ManualResetEventSlim _sEvent;

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
                    if (_workMethod == null) return false;
                    if (IsIPCLogger)
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
            _sEvent.Wait();

            for (int i = 0; i < _recordsCount; i++)
            {
                _logger.Info(_sGuid);
            }

            ((ManualResetEvent) obj).Set();
        }

        internal static void WriteLogIPC(object obj)
        {
            _sEvent.Wait();

            for (int i = 0; i < _recordsCount; i++)
            {
                LFactory.Instance.Write(LogEvent.Info, _sGuid);
                //Thread.Sleep(1000);
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
            if (IsIPCLogger)
            {
                LFactory.Instance.Initialize();
                LFactory.Instance.Write(LogEvent.Info, _sGuid);
            }
            else
            {
                XmlConfigurator.Configure(new FileInfo(@"IPCLogger.TestService.exe.config"));
                _logger.Info(_sGuid);
            }

            _tEvents = new WaitHandle[_parallelOperations];
            for (int i = 0; i < _parallelOperations; i++)
            {
                _tEvents[i] = new ManualResetEvent(false);
            }
            _sEvent = new ManualResetEventSlim(false);

            for (int i = 0; i < _parallelOperations; i++)
            {
                ThreadPool.QueueUserWorkItem(_workMethod, _tEvents[i]);
            }

            _timer = HRTimer.CreateAndStart();
            _sEvent.Set();
            WaitHandle.WaitAll(_tEvents);

            if (IsIPCLogger)
            {
                LFactory.Instance.Flush();
            }
            else
            {
                FlushLog4NetBuffers();
            }
        }

        internal static void TestMethod(int value)
        {
            using (TLSObject tlsObj = TLS.Push())
            {
                tlsObj.SetClosure(() => value);
                LFactory.Instance.WriteLine(LogEvent.Debug, (string) null);
            }
            Console.ReadKey();
            Process.GetCurrentProcess().Kill();
        }

        static void Main(string[] param)
        {
            //using (TLSObject tlsObj = TLS.Push())
            //{
            //    //TLS.CaptureObjectGlobal("XService", new X {Y = "1", Z = 2, G = new Guid()});
            //    TLS.SetClosureGlobal("XService", () => new[] {1, 2, 3});
            //    LFactory.Instance.WriteLine(LogEvent.Debug, (string) null);
            //}

            //using (TLSObject tlsObj = TLS.Push())
            //{
            //    tlsObj.CaptureObject("XService", new X {Y = "1", Z = 2, G = new Guid()}, false);
            //    _timer = HRTimer.CreateAndStart();
            //    tlsObj.SetClosure(() => _timer, () => LFactory.Instance);
            //    for (int i = 0; i < 1; i++)
            //    {
            //        LFactory.Instance.WriteLine(LogEvent.Debug, (string)null);
            //    }
            //    Console.WriteLine(_timer.StopWatch());
            //}

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
