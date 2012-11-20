//Copyright (c) CodeSharp.  All rights reserved.

using System.Collections.Generic;

namespace CodeSharp.EventSourcing
{
    public interface IAggregateEventHandlerProvider : IEventHandlerProvider<AggregateEventHandlerMetaData>
    {
    }
    /// <summary>
    /// 聚合根事件订阅者元数据信息
    /// </summary>
    public class AggregateEventHandlerMetaData : EventHandlerMetaData
    {
        /// <summary>
        /// 能够获取聚合根实例的一个路径集合
        /// </summary>
        public IEnumerable<Path> Paths { get; set; }
    }
}
