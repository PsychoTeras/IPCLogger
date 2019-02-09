using System.Threading;

namespace IPCLogger.Core.Patterns.Base
{
    public sealed class Pattern
    {
        private static int _idCounter;

        public int Id { get; private set; }

        public string Content;

        public bool ImmediateFlush;

        public Pattern(string content, bool immediateFlush)
        {
            Id = Interlocked.Increment(ref _idCounter);
            Content = content;
            ImmediateFlush = immediateFlush;
        }

        public void SetChanged()
        {
            Id = Interlocked.Increment(ref _idCounter);
        }
    }
}
