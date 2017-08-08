using System;
using System.Text;
using IPCLogger.Core.Patterns;
using IPCLogger.Core.Snippets.Base;

namespace IPCLogger.Core.Caches
{
    internal sealed class SnippetsCache
    {
        private SnippetsCache _nextItem;
        private volatile int _sbLastCapacity = 16;

        public string PreContent;

        public string Name;
        public string Params;
        public BaseSnippet Snippet;

        public SnippetsCache CreateNext()
        {
            return _nextItem = new SnippetsCache();
        }

        public string Process(Type callerType, Enum eventType, byte[] data, string text, PFactory pFactory)
        {
            StringBuilder result = new StringBuilder(_sbLastCapacity);
            SnippetsCache record = this;
            do
            {
                if (record.PreContent != null)
                {
                    result.Append(record.PreContent);
                }
                if (record.Snippet != null)
                {
                    string value = record.Snippet.Process(callerType, eventType, record.Name, data, text, 
                        record.Params, pFactory);
                    if (!string.IsNullOrEmpty(value))
                    {
                        result.Append(value);
                    }
                }
            } while ((record = record._nextItem) != null);

            _sbLastCapacity = (result.Capacity + _sbLastCapacity) / 2;

            return result.ToString();
        }
    }
}
