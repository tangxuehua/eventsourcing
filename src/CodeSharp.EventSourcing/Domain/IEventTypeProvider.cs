//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    public interface IEventTypeProvider
    {
        /// <summary>
        /// 判断指定的事件是否是一个支持事件溯源的事件，可溯源事件会被持久化。
        /// </summary>
        bool IsSourcableEvent(Type eventType);
        /// <summary>
        /// 判断指定的事件是否是一个仅仅用来通知的事件，通知事件不会被持久化。
        /// </summary>
        bool IsNotityEvent(Type eventType);
    }
}
