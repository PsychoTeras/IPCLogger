using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using IPCLogger.Core.Loggers.LIPC.FileMap;
using IPCLogger.Core.Proto;

namespace IPCLogger.Core.Loggers.LIPC
{
    public sealed class LIPCView : IDisposable
    {

#region Deletates

        public delegate void OnEvent(LogRecord ev);

#endregion

#region Constants

        private const ushort QUERY_INTERVAL_MSEC = 10;

#endregion

#region Private fields

        private ushort _queryIntervalMsec = QUERY_INTERVAL_MSEC;

        private Timer _poolTimer;
        private MapRingBuffer<LogRecord> _ipcEventRecords;
        private HashSet<long> _processedEventsList;

        private OnEvent _onEvent;

#endregion

#region Properties

        public ushort QueryIntervalMsec
        {
            get { return _queryIntervalMsec; }
            set
            {
                if (_queryIntervalMsec != value)
                {
                    _queryIntervalMsec = Math.Max(value, (ushort) 1);
                    if (_poolTimer != null)
                    {
                        RecreatePoolTimer();
                    }
                }
            }
        }

#endregion

#region Class methods

        public Process[] GetHosts(string processName)
        {
            return Process.GetProcesses().
                Where(p => p.ProcessName.Equals(processName, StringComparison.InvariantCultureIgnoreCase)).
                ToArray();
        }

        private void RecreatePoolTimer()
        {
            if (_poolTimer != null)
            {
                _poolTimer.Dispose();
                _poolTimer = null;
            }

            _poolTimer = new Timer();
            _poolTimer.Elapsed += OnTimer;
            _poolTimer.Interval = _queryIntervalMsec;
            _poolTimer.AutoReset = true;
            _poolTimer.Start();
        }

        public void StartView(Process process, OnEvent onEvent)
        {
            string mmfName = string.Format(@"{0}_{1}", process.ProcessName, process.Id);
            StartView(mmfName, onEvent);
        }

        public void StartView(string customName, OnEvent onEvent)
        {
            StopView();

            _onEvent = onEvent;

            _processedEventsList = new HashSet<long>();
            string hostName = string.Format(@"Global\LIPC~{0}", customName);
            _ipcEventRecords = MapRingBuffer<LogRecord>.View(hostName);

            RecreatePoolTimer();
        }

        public void StopView()
        {
            if (_poolTimer != null)
            {
                _poolTimer.Dispose();
                _poolTimer = null;
            }
            if (_ipcEventRecords != null)
            {
                _ipcEventRecords.Dispose();
                _ipcEventRecords = null;
            }
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            if (_ipcEventRecords.Initialized)
            {
                lock (_processedEventsList)
                {
                    LogRecord[] allEvents = _ipcEventRecords.ToArray();

                    IEnumerable<LogRecord> newEvents = allEvents.Where
                        (
                            ev => !_processedEventsList.Contains(ev.Id)
                        ).
                        OrderBy(ev => ev.Id);
                    foreach (LogRecord record in newEvents)
                    {
                        if (_onEvent != null)
                        {
                            _onEvent(record);
                        }
                        _processedEventsList.Add(record.Id);
                    }

                    _processedEventsList.RemoveWhere
                        (
                            l => allEvents.FirstOrDefault(ev => ev.Id == l).IsEmpty
                        );
                }
            }
        }

        public void Dispose()
        {
            StopView();
        }

#endregion

    }
}