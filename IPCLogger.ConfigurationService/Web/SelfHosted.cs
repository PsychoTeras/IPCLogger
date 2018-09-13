using IPCLogger.ConfigurationService.Web.modules;
using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace IPCLogger.ConfigurationService.Web
{
    internal class SelfHosted
    {

#region Private fields

        private NancyHost _host;
        private int _port = 8080;

        private HostAssetsWatcher _assetsWatcher;

#endregion

#region Properties

        public static SelfHosted Instance { get; } = new SelfHosted();

        public bool Started { get; private set; }

        public string Url => $"http://localhost:{_port}";

        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                if (Started)
                {
                    Restart();
                }
            }
        }

#endregion

#region Ctor

        private SelfHosted() { }

#endregion

#region Private methods

        private string GetLocalIPAddresses()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

#endregion

#region Public methods

        public void Stop()
        {
            if (!Started) return;

            _host?.Dispose();
            _host = null;
            _assetsWatcher?.Dispose();
            _assetsWatcher = null;

            Started = false;
        }

        public void Start()
        {
            if (Started) return;

            if (System.Diagnostics.Debugger.IsAttached)
            {
                _assetsWatcher = new HostAssetsWatcher();
                _assetsWatcher.Start();
            }

            _host = new NancyHost
            (
                new Uri($"http://localhost:{_port}"),
                new Uri($"http://127.0.0.1:{_port}"),
                new Uri($"http://{GetLocalIPAddresses()}:{_port}")
            );
            _host.Start();

            Started = true;
        }

        public void Restart()
        {
            Stop();
            Start();
        }

#endregion

    }

    internal class HostAssetsWatcher : IDisposable
    {
        private string[] _srcFolders;
        private string[] _destFolders;
        private List<FileSystemWatcher> _watchers;
        private Dictionary<string, string> _srcDestFilesMatch;

        public HostAssetsWatcher()
        {
            SetStaticContentFolders();
            FirstStartCopyContent();
        }

        private void SetStaticContentFolders()
        {
            string binPath = Directory.GetCurrentDirectory();
            string appName = Assembly.GetExecutingAssembly().GetName().Name;
            string solutionPath = Path.Combine(Directory.GetParent(binPath).Parent.FullName, appName);
            string[] dirs = BSMain.StaticContentsConventions.
                Select(kv => kv.Value.Replace('/', '\\')).Distinct().ToArray();
            _srcFolders = dirs.Select(dir => solutionPath + dir).ToArray();
            _destFolders = dirs.Select(dir => binPath + dir).ToArray();
        }

        private void FirstStartCopyContent()
        {
            string GetMatchFile(string destFolder, string[] destFiles, string srcFolder, string srcFile, out bool targetFileExists)
            {
                string relFileName = srcFile.Replace(srcFolder + "\\", "");
                string targetFileName = Path.Combine(destFolder, relFileName);
                targetFileExists = destFiles.Contains(targetFileName);
                return targetFileName;
            }

            _srcDestFilesMatch = new Dictionary<string, string>();

            for (int i = 0; i < _srcFolders.Length; i++)
            {
                string srcFolder = _srcFolders[i];
                if (!Directory.Exists(srcFolder)) continue;

                string[] srcFiles = Directory.EnumerateFiles(srcFolder, "*.*", SearchOption.AllDirectories).ToArray();

                string destFolder = _destFolders[i];
                Directory.CreateDirectory(destFolder);
                string[] destFiles = Directory.EnumerateFiles(destFolder, "*.*", SearchOption.AllDirectories).ToArray();

                foreach (string srcFile in srcFiles)
                {
                    string targetFile = GetMatchFile(destFolder, destFiles, srcFolder, srcFile, out var targetFileExists);
                    if (!targetFileExists || File.GetLastWriteTime(srcFile) != File.GetLastWriteTime(targetFile))
                    {
                        string targetPath = Path.GetDirectoryName(targetFile);
                        Directory.CreateDirectory(targetPath);
                        File.Copy(srcFile, targetFile, true);
                    }

                    _srcDestFilesMatch.Add(srcFile, targetFile);
                }
            }
        }

        public void Start()
        {
            _watchers = new List<FileSystemWatcher>();
            foreach (string srcFolder in _srcFolders)
            {
                if (!Directory.Exists(srcFolder)) continue;

                FileSystemWatcher watcher = new FileSystemWatcher(srcFolder);
                watcher.IncludeSubdirectories = true;
                watcher.Changed += WatcherFileChanged;
                watcher.Renamed += WatcherFileChanged;
                watcher.EnableRaisingEvents = true;
                _watchers.Add(watcher);
            }
        }

        private void WatcherFileChanged(object sender, FileSystemEventArgs e)
        {
            if (_srcDestFilesMatch.TryGetValue(e.FullPath, out var destFile))
            {
                int tryCnt = 0;
                while (tryCnt < 3)
                {
                    try
                    {
                        File.Copy(e.FullPath, destFile, true);
                        tryCnt = 3;
                    }
                    catch
                    {
                        Thread.Sleep(100);
                        tryCnt++;
                    }
                }
            }
        }

        public void Stop()
        {
            foreach (FileSystemWatcher watcher in _watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            _watchers = null;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
