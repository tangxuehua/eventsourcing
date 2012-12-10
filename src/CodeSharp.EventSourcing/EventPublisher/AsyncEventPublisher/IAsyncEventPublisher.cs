//Copyright (c) CodeSharp.  All rights reserved.

using System.Collections.Generic;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// Represents an async event publisher to publish sourcable events.
    /// </summary>
    public interface IAsyncEventPublisher
    {
        /// <summary>
        /// 分发单个事件
        /// </summary>
        void PublishEvent(object evnt);
        /// <summary>
        /// 分发多个事件
        /// </summary>
        void PublishEvents(IEnumerable<object> evnts);
        /// <summary>
        /// 启动事件发布者
        /// </summary>
        void Start();
    }
}
