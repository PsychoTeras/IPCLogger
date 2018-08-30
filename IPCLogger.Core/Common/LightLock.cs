using System;
using System.Threading;

namespace IPCLogger.Core.Common
{
    internal class LightLock
    {
        private const int SPINS_COUNT = 10;
        private const int SPINS_COUNT_MID = 5;
        private const int THREAD_SLEEP_0_MOD = 5;
        private const int THREAD_SLEEP_1_MOD = 20;

        private const int THREAD_VISIT_BOOK_INIT_SIZE = 16;
        private const float THREAD_VISIT_BOOK_GROW_F = 1.5f;

        private byte[] _threadVisitBook = new byte[THREAD_VISIT_BOOK_INIT_SIZE];
        private volatile int _threadVisitBookSize = THREAD_VISIT_BOOK_INIT_SIZE;

        private int _waiter;

        private bool SetTheCurrentThreadHasVisit()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (_threadVisitBookSize < threadId)
            {
                lock (_threadVisitBook)
                {
                    if (_threadVisitBookSize < threadId)
                    {
                        int newSize = (int) (threadId*THREAD_VISIT_BOOK_GROW_F);
                        Array.Resize(ref _threadVisitBook, newSize);
                        _threadVisitBookSize = newSize;
                    }
                }
            }

            if (_threadVisitBook[--threadId] == 1)
            {
                return false;
            }

            _threadVisitBook[threadId] = 1;
            return true;
        }

        public void WaitOne(bool @lock = true)
        {
            if (!@lock || !SetTheCurrentThreadHasVisit()) return;

            int loops = 0;
            while (Interlocked.CompareExchange(ref _waiter, 1, 0) != 0)
            {
                if (loops++ < SPINS_COUNT)
                {
                    if (loops == SPINS_COUNT_MID)
                    {
                        Thread.Yield();
                    }
                    else
                    {
                        Thread.SpinWait(Constants.ProcessorsCount);
                    }
                }
                else if (loops%THREAD_SLEEP_0_MOD == 0)
                {
                    Thread.Sleep(0);
                }
                else if (loops%THREAD_SLEEP_1_MOD == 0)
                {
                    Thread.Sleep(1);
                }
                else
                {
                    Thread.Yield();
                }
            }
        }

        public void Set(bool unlock = true)
        {
            if (unlock)
            {
                _threadVisitBook[Thread.CurrentThread.ManagedThreadId - 1] = 0;
                Interlocked.CompareExchange(ref _waiter, 0, 1);
            }
        }
    }
}
