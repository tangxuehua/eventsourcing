//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 定义用于存储聚合根可溯源事件的接口
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// 持久化给定的可溯源事件
        /// </summary>
        /// <param name="evnts">要持久化的可溯源事件</param>
        void StoreEvents(IEnumerable<SourcableEvent> evnts);
        /// <summary>
        /// 获取指定聚合根上发生的可溯源事件
        /// </summary>
        /// <param name="aggregateRootId">聚合根ID</param>
        /// <param name="aggregateRootType">聚合根类型</param>
        /// <param name="minVersion">事件的起始版本号</param>
        /// <param name="maxVersion">事件的结束版本号</param>
        /// <returns>返回符合条件的可溯源事件</returns>
        IEnumerable<SourcableEvent> GetEvents(string aggregateRootId, Type aggregateRootType, long minVersion, long maxVersion);
    }
}
