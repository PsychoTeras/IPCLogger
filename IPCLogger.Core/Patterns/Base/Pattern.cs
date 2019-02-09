using System.Threading;

namespace IPCLogger.Core.Patterns.Base
{
    public sealed class Pattern
    {
        private static int _idCounter;

        public int Id { get; }

        public string Description { get; }

        public string Content { get; }

        public bool ImmediateFlush { get; }

        public Pattern(string description, string content, bool immediateFlush)
        {
            Id = Interlocked.Increment(ref _idCounter);
            Description = description;
            Content = content;
            ImmediateFlush = immediateFlush;
        }
    }
}
