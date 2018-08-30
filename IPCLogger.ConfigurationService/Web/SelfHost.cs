using Nancy.Hosting.Self;
using System;

namespace IPCLogger.ConfigurationService.Web
{
    public class SelfHost
    {
        private NancyHost _host;
        private int _port = 8080;

        public bool Started { get; private set; }

        public static SelfHost Instance = new SelfHost();

        private SelfHost() { }

        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
                if (Started)
                {
                    Restart();
                }
            }
        }

        public void Stop()
        {
            if (!Started) return;

            _host.Dispose();
            _host = null;
        }

        public void Start()
        {
            if (Started) return;

            if (_host != null)
            {
                _host.Dispose();
            }

            _host = new NancyHost(new Uri($"http://localhost:{_port}"));
            _host.Start();
        }

        public void Restart()
        {
            Stop();
            Start();
        }
    }
}
