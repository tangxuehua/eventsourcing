//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 表示持久化事件时出现的并发异常
    /// </summary>
    public class ConcurrencyException : Exception
    {
        public const string DefaultConcurrencyExceptionMessage = "Concurrency exception happened when persisting sourcable events. Please retry to save the changes again.";

        public ConcurrencyException() : base() { }
        public ConcurrencyException(string message) : base(message) { }
        public ConcurrencyException(string message, Exception innerException) : base(message, innerException) { }
        public ConcurrencyException(string message, params object[] args) : base(string.Format(message, args)) { }
    }
}
