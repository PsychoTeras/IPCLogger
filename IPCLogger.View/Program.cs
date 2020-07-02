using IPCLogger.Loggers.LConsole;
using IPCLogger.Loggers.LIPC;
using IPCLogger.Proto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace IPCLogger.View
{
    static class Program
    {

#region Definitions

        enum EventNum { }

#endregion

#region Private fields

        private static readonly Dictionary<string, string> _startupParams = 
            new Dictionary<string, string>();

        private static LConsole _console;

#endregion

#region Properties

        private static string CustomName
        {
            get
            {
                return _startupParams.ContainsKey("-name")
                    ? _startupParams["-name"]
                    : null;
            }
        }

        private static string CustomConfig
        {
            get
            {
                return _startupParams.ContainsKey("-config")
                    ? _startupParams["-config"]
                    : null;
            }
        }

        private static string ProcessName { get; set; }

#endregion

#region Class methods

        static void ReadStartupParams(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i][0] == '-' && i + 1 < args.Length && args[i + 1][0] != '-')
                {
                    _startupParams.Add(args[i].ToLower(), args[++i]);
                }
                else
                {
                    string param = args[i].ToLower();
                    _startupParams.Add(param, string.Empty);
                    if (param[0] != '-')
                    {
                        ProcessName = param;
                    }
                }
            }
        }

        static Process GetHostProcess(LIPCView logger)
        {
            Process[] hosts = logger.GetHosts(ProcessName);
            switch (hosts.Length)
            {
                case 0:
                    return null;
                case 1:
                    return hosts[0];
                default:
                    using (frmSelectProcess form = new frmSelectProcess())
                    {
                        if (form.Execute(hosts))
                        {
                            return form.SelectedHost;
                        }
                    }
                    break;
            }
            return null;
        }

        static void InitializeLogger()
        {
            _console = new LConsole(true);
            _console.Settings = new LConsoleSettings(typeof(LConsole), null)
            {
                Enabled = true,
                Highlights = new LConsoleSettings.HighlightSettings()
            };

            string customConfig = CustomConfig;
            if (!string.IsNullOrEmpty(customConfig))
            {
                string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string file = Path.Combine(path, customConfig);
                _console.Settings.Setup(file);
            }

            _console.Initialize();
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            ReadStartupParams(args);

            InitializeLogger();

            using (LIPCView logger = new LIPCView())
            {
                bool started = true;
                if (!string.IsNullOrEmpty(CustomName))
                {
                    logger.StartView(CustomName, OnEvent);
                }
                else
                {
                    Process host = GetHostProcess(logger);
                    started = host != null;
                    if (started)
                    {
                        Console.Title = $"{host.ProcessName} [{host.Id}]";
                        logger.StartView(host, OnEvent);
                    }
                }

                if (started)
                {
                    Console.ReadLine();
                }

                Environment.Exit(0);
            }
        }

        private static void OnEvent(LogItem ev)
        {
            _console.Write((EventNum)ev.Type, ev.Message);
        }

#endregion

    }
}
