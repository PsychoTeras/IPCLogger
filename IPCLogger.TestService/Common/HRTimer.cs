using System.Runtime.InteropServices;

namespace IPCLogger.TestService.Common
{
    class HRTimer
    {
        private long _start;
        private double _result;
        private readonly long _frequency;

        [DllImport("kernel32.dll")]
        private static extern int QueryPerformanceCounter(ref long count);

        [DllImport("kernel32.dll")]
        private static extern int QueryPerformanceFrequency(ref long frequency);

        public HRTimer()
        {
            _start = 0;
            QueryPerformanceFrequency(ref _frequency);
        }

        public double Result
        {
            get { return _result; }
        }

        public static HRTimer CreateAndStart()
        {
            HRTimer hr = new HRTimer();
            hr.StartWatch();
            return hr;
        }

        public void StartWatch()
        {
            _result = 0;
            QueryPerformanceCounter(ref _start);
        }

        public double StopWatch()
        {
            long stop = 0;
            QueryPerformanceCounter(ref stop);
            _result = (double)(stop - _start)/_frequency * 1000;
            return _result;
        }
    }
}
