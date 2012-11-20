//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 快照创建策略实现类，此策略告诉框架总是不需要创建快照
    /// </summary>
    public class NoSnapshotPolicy : ISnapshotPolicy
    {
        /// <summary>
        /// 总是返回False，表示总是不需要创建快照
        /// </summary>
        public bool ShouldCreateSnapshot(AggregateRoot aggregateRoot)
        {
            return false;
        }
    }
}
