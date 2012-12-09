//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 聚合根的快照类定义
    /// </summary>
    public class Snapshot
    {
        #region Constructors

        public Snapshot()
        {
            UniqueId = Guid.NewGuid().ToString();
        }
        public Snapshot(Type aggregateRootType, string aggregateRootId, long version, object data, DateTime createdTime) : this()
        {
            AggregateRootType = aggregateRootType;
            AggregateRootId = aggregateRootId;
            Version = version;
            Data = data;
            CreatedTime = createdTime;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 唯一标识
        /// </summary>
        public virtual string UniqueId { get; set; }
        /// <summary>
        /// 快照对应的聚合根的类型
        /// </summary>
        public virtual Type AggregateRootType { get; set; }
        /// <summary>
        /// 快照对应的聚合根的类型对应的名称
        /// </summary>
        public virtual string AggregateRootName { get; set; }
        /// <summary>
        /// 快照对应的聚合根的Id
        /// </summary>
        public virtual string AggregateRootId { get; set; }
        /// <summary>
        /// 快照创建时聚合根的版本
        /// </summary>
        public virtual long Version { get; set; }
        /// <summary>
        /// 快照包含的数据的类型对应的名称
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 快照包含的数据
        /// </summary>
        public virtual object Data { get; set; }
        /// <summary>
        /// 快照包含的数据的字符串形式
        /// </summary>
        public virtual string SerializedData { get; set; }
        /// <summary>
        /// 快照创建时间
        /// </summary>
        public virtual DateTime CreatedTime { get; set; }

        #endregion

        #region Public Methohds

        /// <summary>
        /// 返回快照的基本数据有效性
        /// </summary>
        public bool IsValid()
        {
            return
                AggregateRootType != null && typeof(AggregateRoot).IsAssignableFrom(AggregateRootType)
                && AggregateRootId != null
                && Version > 0
                && Data != null;
        }

        #endregion
    }
}
