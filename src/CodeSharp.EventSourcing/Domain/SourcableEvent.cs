//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// ISourcableEvent泛型接口定义，
    /// 该接口允许用户在聚合根内生成SourcableEvent时可以返回自己实现的SourcableEvent实例
    /// </summary>
    public interface ISourcableEvent<TAggregateRoot> where TAggregateRoot : AggregateRoot
    {
        SourcableEvent Initialize(TAggregateRoot aggregateRoot, object evnt, long version);
    }
    /// <summary>
    /// 描述聚合根上某个已发生的可溯源事件的相关信息
    /// </summary>
    public abstract class SourcableEvent
    {
        #region Public Properties

        /// <summary>
        /// 唯一标识
        /// </summary>
        public virtual string UniqueId { get; set; }
        /// <summary>
        /// 事件所属聚合根的Id
        /// </summary>
        public virtual string AggregateRootId { get; set; }
        /// <summary>
        /// 事件所属聚合根的类型
        /// </summary>
        public virtual Type AggregateRootType { get; set; }
        /// <summary>
        /// 事件所属聚合根的类型对应的名称
        /// </summary>
        public virtual string AggregateRootName { get; set; }
        /// <summary>
        /// 事件的版本号
        /// </summary>
        public virtual long Version { get; set; }
        /// <summary>
        /// 用户定义的事件对象的类型对应的名称
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 用户定义的原始事件对象
        /// </summary>
        public virtual object RawEvent { get; set; }
        /// <summary>
        /// 用户定义的事件对象的字符串形式
        /// </summary>
        public virtual string Data { get; set; }
        /// <summary>
        /// 事件的发生时间
        /// </summary>
        public virtual DateTime OccurredTime { get; set; }

        #endregion
    }
    public class SourcableEvent<TAggregateRoot> : SourcableEvent, ISourcableEvent<TAggregateRoot> where TAggregateRoot : AggregateRoot
    {
        #region Public Properties

        /// <summary>
        /// 唯一标识
        /// </summary>
        public new virtual string UniqueId
        {
            get
            {
                return base.UniqueId;
            }
            set
            {
                base.UniqueId = value;
            }
        }
        /// <summary>
        /// 事件所属聚合根的Id
        /// </summary>
        public new virtual string AggregateRootId
        {
            get
            {
                return base.AggregateRootId;
            }
            set
            {
                base.AggregateRootId = value;
            }
        }
        /// <summary>
        /// 事件所属聚合根的类型对应的名称
        /// </summary>
        public new virtual string AggregateRootName
        {
            get
            {
                return base.AggregateRootName;
            }
            set
            {
                base.AggregateRootName = value;
            }
        }
        /// <summary>
        /// 事件的版本号
        /// </summary>
        public new virtual long Version
        {
            get
            {
                return base.Version;
            }
            set
            {
                base.Version = value;
            }
        }
        /// <summary>
        /// 用户定义的事件对象的类型对应的名称
        /// </summary>
        public new virtual string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }
        /// <summary>
        /// 用户定义的事件对象的字符串形式
        /// </summary>
        public new virtual string Data
        {
            get
            {
                return base.Data;
            }
            set
            {
                base.Data = value;
            }
        }
        /// <summary>
        /// 事件的发生时间
        /// </summary>
        public new virtual DateTime OccurredTime
        {
            get
            {
                return base.OccurredTime;
            }
            set
            {
                base.OccurredTime = value;
            }
        }

        #endregion

        public virtual SourcableEvent Initialize(TAggregateRoot aggregateRoot, object evnt, long version)
        {
            UniqueId = Guid.NewGuid().ToString();
            AggregateRootId = aggregateRoot.UniqueId;
            AggregateRootType = aggregateRoot.GetType();
            RawEvent = evnt;
            Version = version;
            OccurredTime = DateTime.Now;
            return this;
        }
    }
}
