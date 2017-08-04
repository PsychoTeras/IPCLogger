using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using IPCLogger.Core.Common;
using IPCLogger.Core.Proto;

namespace IPCLogger.Core.Loggers.LIPC.FileMap
{
    internal unsafe class MapRingBuffer<TItem> : IEnumerable<TItem>, IDisposable
        where TItem : struct, ISerializable
    {

#region Constants

        private const int DEF_ITEM_SIZE = 512;

#endregion

#region Private fields

        private string _name;
        private bool _viewMode;
        private MemoryMappedFile _map;
        private MapViewStream _stream;

        private int _maxCount;

        private MapRingBufferHeader* _header;
        private int _headerSize;
        
        private byte* _itemBuffer;
        private int _itemBufferSize;

        private LightLock _lockObj;

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
            _lockObj = new LightLock();

            _headerSize = Marshal.SizeOf(typeof (MapRingBufferHeader));

            _name = name;
            _maxCount = maxCount;
            _viewMode = viewMode;

            Reinitialize();
        }

#endregion

#region Static methods

        public static MapRingBuffer<TItem> Host(string name, int maxCount)
        {
            return new MapRingBuffer<TItem>(name, maxCount, false);
        }

        public static MapRingBuffer<TItem> View(string name)
        {
            return new MapRingBuffer<TItem>(name, 0, true);
        }

#endregion

#region Class methods

        private void PrepareItemBuffer(int itemSize)
        {
            if (_itemBufferSize < itemSize)
            {
                _itemBuffer = (byte*) (_itemBufferSize != 0
                    ? Win32.HeapReAlloc(_itemBuffer, itemSize, false)
                    : Win32.HeapAlloc(itemSize, false));
                _itemBufferSize = itemSize;
            }
        }

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
                    _threadInit?.Join();
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
                int size = DEF_ITEM_SIZE * _maxCount;
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
                    _map.Size = size;
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
            Marshal.StructureToPtr(header, memPtr, false);
            _stream.Write(memPtr.ToPointer(), 0, _headerSize);
            Marshal.DestroyStructure(memPtr, typeof(MapRingBufferHeader));
            Marshal.FreeHGlobal(memPtr);
        }

        private void WriteItem(TItem item, int prewItemPosition)
        {
            int itemSize = 0;
            item.Serialize(_itemBuffer, ref itemSize);
            Serializer.Write(_itemBuffer, prewItemPosition, ref itemSize);

            _header->CurrentItemPosition += _header->CurrentItemSize;
            int position = _headerSize + _header->CurrentItemPosition;
            _stream.Write(_itemBuffer, position, itemSize);
            _header->CurrentItemSize = itemSize;
        }

        public void Write(TItem item)
        {
            _lockObj.WaitOne();

            _header->Updating = true;
            _stream.FlushHeader();

            int currentItemPosition = _header->CurrentItemPosition;
            if (++_header->CurrentIndex == _header->MaxCount)
            {
                _header->CurrentIndex = 0;
                _header->CurrentItemPosition = 0;
                _header->CurrentItemSize = 0;
            }

            PrepareItemBuffer(item.SizeOf + sizeof(int));
            WriteItem(item, currentItemPosition);

            if (_header->Count < _header->MaxCount)
            {
                _header->Count++;
            }
            _header->Updating = false;

            int flushSize = _headerSize + _header->CurrentItemPosition + _header->CurrentItemSize;
            _stream.FlushFromBeginning(flushSize);

            _lockObj.Set();
        }

        private TItem Read(ref int pos)
        {
            TItem item = new TItem();
            byte* p = (byte*) _stream.GetElemPtr(_headerSize);
            item.Deserialize(p, ref pos);

            int prewItemPosition;
            Serializer.Read(p, out prewItemPosition, ref pos);
            pos = prewItemPosition;

            return item;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            _lockObj.WaitOne();

            while (Header.Updating)
            {
                Thread.Sleep(1);
            }

            try
            {
                int pos = Header.CurrentItemPosition;
                for (int i = 0; i < Header.Count; i++)
                {
                    TItem item = Read(ref pos);
                    yield return item;
                }
            }
            finally
            {
                _lockObj.Set();
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

            if (_itemBufferSize != 0)
            {
                Win32.HeapFree(_itemBuffer);
            }

            _initialized = false;
        }

#endregion

    }
}
