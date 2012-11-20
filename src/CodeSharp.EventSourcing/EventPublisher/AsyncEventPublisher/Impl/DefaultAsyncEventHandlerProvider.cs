//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 默认的异步事件处理函数元数据提供者
    /// </summary>
    public class DefaultAsyncEventHandlerProvider : AttributeBasedEventHandlerProvider<AsyncEventHandlerAttribute, EventHandlerMetaData>, IAsyncEventHandlerProvider
    {
        protected override EventHandlerMetaData CreateMetaData(Type subscriberType, MethodInfo eventHandler)
        {
            return new EventHandlerMetaData { SubscriberType = subscriberType, EventHandler = eventHandler };
        }
    }
}
