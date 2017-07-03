using System.Threading;

namespace IPCLogger.Core.Common
{
    public sealed class LightLock
    {
        private int _waiter;

        public void WaitOne(bool @lock = true)
        {
            while (@lock && Interlocked.CompareExchange(ref _waiter, 1, 0) != 0)
            {
                Thread.Sleep(0);
            }
        }

        public void Set(bool unlock = true)
        {
            if (unlock)
            {
                Interlocked.CompareExchange(ref _waiter, 0, 1);
            }
        }
    }
}
