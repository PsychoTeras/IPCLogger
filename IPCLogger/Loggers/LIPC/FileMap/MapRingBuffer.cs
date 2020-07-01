using IPCLogger.Common;
using IPCLogger.Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace IPCLogger.Loggers.LIPC.FileMap
{
    internal unsafe class MapRingBuffer<TItem> : IEnumerable<TItem>, IDisposable
        where TItem : struct, ISerializable
    {

#region Private fields

        private string _name;
        private bool _viewMode;
        private MemoryMappedFile _map;

        private int _maxCount;

        private volatile MapRingBufferHeader* _header;
        private volatile MapRingBufferHeader* _backingHeader;
        private int _headerSize;
        
        private byte* _itemBuffer;
        private int _itemBufferSize;

        private Thread _threadInit;
        private volatile bool _initialized;

#endregion

#region Properties

        private MapRingBufferHeader Header
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
                while (Thread.CurrentThread.IsAlive && _map == null)
                {
                    _map = _viewMode
                        ? MemoryMappedFile.Open(_name, MapAccess.FileMapRead)
                        : MemoryMappedFile.Create(_name, MapProtection.PageReadWrite);
                    if (_map == null)
                    {
                        Thread.Sleep(1);
                    }
                }

                if (_map == null)
                {
                    return;
                }

                if (!_viewMode)
                {
                    WriteHeader();
                    CreateBackingHeader();
                }

                _header = (MapRingBufferHeader*) _map.GetItemPtr(0);

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
            _map.Write(memPtr.ToPointer(), 0, _headerSize);
            Marshal.DestroyStructure(memPtr, typeof(MapRingBufferHeader));
            Marshal.FreeHGlobal(memPtr);
        }

        private unsafe void CreateBackingHeader()
        {
            _backingHeader = (MapRingBufferHeader*)Win32.HeapAlloc(_headerSize);
        }

        private void PrepareItemBuffer(int itemSize)
        {
            if (_itemBufferSize < itemSize)
            {
                _itemBuffer = (byte*)(_itemBufferSize != 0
                    ? Win32.HeapReAlloc(_itemBuffer, itemSize, false)
                    : Win32.HeapAlloc(itemSize, false));
                _itemBufferSize = itemSize;
            }
        }

        private int WriteItem(ref TItem item, MapRingBufferHeader* header, int prewItemPosition)
        {
            int itemSize = 0;
            item.Serialize(_itemBuffer, ref itemSize);
            Serializer.Write(_itemBuffer, prewItemPosition, ref itemSize);

            int itemPosition = _headerSize + header->CurrentItemPosition;
            _map.Write(_itemBuffer, itemPosition, itemSize);

            return itemSize;

        }

        public void Write(ref TItem item)
        {
            Win32.Copy(_backingHeader, _header, _headerSize);
            int prewItemPosition = _backingHeader->CurrentItemPosition;

            if (++_backingHeader->CurrentIndex == _backingHeader->MaxCount)
            {
                _backingHeader->CurrentIndex = 0;
                _backingHeader->CurrentItemPosition = 0;
                _backingHeader->CurrentItemSize = 0;
            }
            else
            {
                _backingHeader->CurrentItemPosition += _backingHeader->CurrentItemSize;
            }

            if (_backingHeader->Count < _backingHeader->MaxCount)
            {
                _backingHeader->Count++;
            }

            PrepareItemBuffer(item.SizeOf + sizeof(int));
            _backingHeader->CurrentItemSize = WriteItem(ref item, _backingHeader, prewItemPosition);

            Win32.Copy(_header, _backingHeader, _headerSize);
        }

        private TItem Read(ref TItem item, ref int pos)
        {
            byte* p = (byte*) _map.GetItemPtr(_headerSize);
            item.Deserialize(p, ref pos);

            Serializer.Read(p, out int prewItemPosition, ref pos);
            pos = prewItemPosition;

            return item;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            int pos = Header.CurrentItemPosition;
            int count = Header.Count;
            for (int i = 0; i < count; i++)
            {
                TItem item = new TItem();
                Read(ref item, ref pos);
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Flush()
        {
            _map.Flush();
        }

        public override string ToString()
        {
            return $"Count {Header.Count}";
        }

        public void Dispose()
        {
            if (_threadInit != null && Thread.CurrentThread != _threadInit)
            {
                _threadInit.Abort();
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
