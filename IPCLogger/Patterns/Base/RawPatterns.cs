using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IPCLogger.Patterns.Base
{
    internal class RawPatterns
    {

#region Public fields

        public string EventName;
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

        public RawPatterns(string eventName) : this()
        {
            EventName = eventName;
        }

#endregion

    }
}
