using System;
using System.ServiceProcess;

namespace IPCLogger.TestService
{
    public delegate void OnServiceStart(string[] args);
    public partial class CommonService : ServiceBase
    {
        private OnServiceStart _serviceStart;
        private Action _serviceStop;

        public CommonService()
        {
            InitializeComponent();
        }

        public CommonService(OnServiceStart serviceStart) : this(serviceStart, null) { }

        public CommonService(OnServiceStart serviceStart, Action serviceStop) : this()
        {
            _serviceStart = serviceStart;
            _serviceStop = serviceStop;
        }

        protected override void OnStart(string[] args)
        {
            _serviceStart?.Invoke(args);
        }

        protected override void OnStop()
        {
            _serviceStop?.Invoke();
        }
    }
}
