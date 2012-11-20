//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Linq;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 默认的聚合根事件处理函数元数据提供者
    /// </summary>
    public class DefaultAggregateEventHandlerProvider : AttributeBasedEventHandlerProvider<AggregateEventHandlerAttribute, AggregateEventHandlerMetaData>, IAggregateEventHandlerProvider
    {
        protected override AggregateEventHandlerMetaData CreateMetaData(Type subscriberType, MethodInfo eventHandler)
        {
            return new AggregateEventHandlerMetaData
            {
                SubscriberType = subscriberType,
                EventHandler = eventHandler,
                Paths = (eventHandler.GetCustomAttributes(typeof(AggregateEventHandlerAttribute), false).Single() as AggregateEventHandlerAttribute).Paths
            };
        }
    }
}
