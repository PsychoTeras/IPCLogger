﻿using System;

namespace IPCLogger.Loggers.LMail
{
    internal class MailData
    {

#region Public fields

        public Type CallerType;
        public Enum EventType;
        public string Message;

#endregion

#region Ctor

        public MailData(Type callerType, Enum eventType, string message)
        {
            CallerType = callerType;
            EventType = eventType;
            Message = message;
        }

#endregion

    }
}
