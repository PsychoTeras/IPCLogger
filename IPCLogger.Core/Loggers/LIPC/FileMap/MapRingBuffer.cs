using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace IPCLogger.Core.Loggers.LIPC.FileMap
{
    internal unsafe class MapRingBuffer<TValue> : IEnumerable<TValue>, IDisposable
        where TValue : struct
    {

#region Private fields

        private string _name;
        private bool _viewMode;
        private MemoryMappedFile _map;
        private MapViewStream _stream;

        private int _dataSize;
        private int _maxCount;
        private IntPtr _memPtr;
        private void* _pMemPtr;

        private int _headerSize;
        private MapRingBufferHeader* _header;

        private object _lockObject;

        private Thread _threadInit;
        private volatile bool _initialized;

#endregion

#region Properties

        public MapRingBufferHeader Header
        {
            get { return _header != null ? *_header : default(MapRingBufferHeader); }
        }

        public bool Initialized
        {
            get { return _initialized; }
        }

#endregion

#region Ctor

        MapRingBuffer(string name, int maxCount, bool viewMode)
        {
            _lockObject = new object();

            _headerSize = Marshal.SizeOf(typeof (MapRingBufferHeader));

            _dataSize = Marshal.SizeOf(typeof (TValue));
            _memPtr = Marshal.AllocHGlobal(_dataSize);
            _pMemPtr = _memPtr.ToPointer();

            _name = name;
            _maxCount = maxCount;
            _viewMode = viewMode;

            Reinitialize();
        }

#endregion

#region Static methods

        public static MapRingBuffer<TValue> Host(string name, int maxCount)
        {
            return new MapRingBuffer<TValue>(name, maxCount, false);
        }

        public static MapRingBuffer<TValue> View(string name)
        {
            return new MapRingBuffer<TValue>(name, 0, true);
        }

#endregion

#region Class methods

        private void Reinitialize()
        {
            if (_initialized)
            {
                Dispose();
            }

            _threadInit = new Thread(DoInitialize);
            _threadInit.IsBackground = true;
            _threadInit.Start();

            if (!_viewMode)
            {
                do
                {
                    if (_threadInit != null)
                    {
                        _threadInit.Join();
                    }
                    if (!_initialized)
                    {
                        Thread.Sleep(1);
                    }
                } while (!_initialized);
            }
        }

        private void DoInitialize()
        {
            try
            {
                int size = _dataSize*_maxCount;
                while (Thread.CurrentThread.IsAlive && _map == null)
                {
                    _map = _viewMode
                        ? MemoryMappedFile.Open(_name, MapAccess.FileMapRead)
                        : MemoryMappedFile.Create(_name, size, MapProtection.PageReadWrite);
                    if (_map == null)
                    {
                        Thread.Sleep(1);
                    }
                }

                if (_map == null)
                {
                    return;
                }

                _stream = _map.MapAsStream();

                if (!_viewMode)
                {
                    WriteHeader();
                }

                _header = (MapRingBufferHeader*) _stream.GetElemPtr(0);

                if (_viewMode)
                {
                    _map.Size = _dataSize*_header->MaxCount;
                }

                _initialized = true;
            }
            catch
            {
                Dispose();
            }
            finally
            {
                _threadInit = null;
            }
        }

        private void WriteHeader()
        {
            MapRingBufferHeader header = new MapRingBufferHeader(_maxCount);
            IntPtr memPtr = Marshal.AllocHGlobal(_headerSize);
            Marshal.StructureToPtr(header, memPtr, true);
            _stream.Write(memPtr.ToPointer(), 0, _headerSize);
            Marshal.DestroyStructure(memPtr, typeof(MapRingBufferHeader));
            Marshal.FreeHGlobal(memPtr);
        }

        private void WriteTValue(TValue value, int index)
        {
            Marshal.StructureToPtr(value, _memPtr, true);
            int position = _headerSize + index * _dataSize;
            _stream.Write(_pMemPtr, position, _dataSize);
        }

        private TValue? Read(int index)
        {
            void* value = _stream.GetElemPtr(_headerSize + (index*_dataSize));
            return (TValue) Marshal.PtrToStructure(new IntPtr(value), typeof (TValue));
        }

        public void Add(TValue value)
        {
            lock (_lockObject)
            {
                _header->Updating = true;
                _stream.FlushHeader();

                if (++_header->CurrentIndex == _header->MaxCount)
                {
                    _header->CurrentIndex = 0;
                }

                WriteTValue(value, _header->CurrentIndex);

                if (_header->Count < _header->MaxCount)
                {
                    _header->Count++;
                }
                _header->Updating = false;
                _stream.Flush();
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            lock (_lockObject)
            {
                while (Header.Updating)
                {
                    Thread.Sleep(0);
                }

                int max = Header.Count, cur = Header.CurrentIndex;
                for (int i = cur; i > -(max - cur); i--)
                {
                    int idx = i >= 0 ? i : max + i;
                    TValue? value = Read(idx);
                    if (value.HasValue)
                    {
                        yield return value.Value;
                    }
                    else
                    {
                        Reinitialize();
                        break;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Format("Count {0}", Header.Count);
        }

        public void Dispose()
        {
            if (_threadInit != null && Thread.CurrentThread != _threadInit)
            {
                _threadInit.Abort();
            }

            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }

            if (_map != null)
            {
                _map.Dispose();
                _map = null;
            }

            if (_memPtr != IntPtr.Zero)
            {
                Marshal.DestroyStructure(_memPtr, typeof (TValue));
                Marshal.FreeHGlobal(_memPtr);
                _memPtr = IntPtr.Zero;
            }

            _initialized = false;
        }

#endregion

    }
}
