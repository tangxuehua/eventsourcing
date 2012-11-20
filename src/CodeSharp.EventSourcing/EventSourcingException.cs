//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 表示EventSourcing框架内的异常
    /// </summary>
    public class EventSourcingException : Exception
    {
        public EventSourcingException() : base() { }
        public EventSourcingException(string message) : base(message) { }
        public EventSourcingException(string message, Exception innerException) : base(message, innerException) { }
        public EventSourcingException(string message, params object[] args) : base(string.Format(message, args)) { }
    }
}
