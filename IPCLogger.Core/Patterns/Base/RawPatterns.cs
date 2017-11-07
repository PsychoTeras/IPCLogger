using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IPCLogger.Core.Patterns.Base
{
    sealed class RawPatterns
    {

#region Public fields

        public string Event;
        public Dictionary<Regex, Pattern> Masked;
        public Dictionary<string, Pattern> Strong;

#endregion

#region Properties

        public bool Empty
        {
            get { return Strong.Count == 0 && Masked.Count == 0; }
        }

#endregion

#region Ctor

        public RawPatterns()
        {
            Masked = new Dictionary<Regex, Pattern>();
            Strong = new Dictionary<string, Pattern>();
        }

        public RawPatterns(string @event) : this()
        {
            Event = @event;
        }

#endregion

    }
}
