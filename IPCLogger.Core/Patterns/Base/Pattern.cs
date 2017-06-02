using System.Threading;

namespace IPCLogger.Core.Patterns.Base
{
    public sealed class Pattern
    {
        private static int _idCounter;

        public int Id;
        public string Content;

        public Pattern(string content)
        {
            Id = Interlocked.Increment(ref _idCounter);
            Content = content;
        }

        public void Changed()
        {
            Id = Interlocked.Increment(ref _idCounter);
        }
    }
}
