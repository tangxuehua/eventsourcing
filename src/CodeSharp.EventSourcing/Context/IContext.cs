//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// Represents main entry point for the event sourcing framework.
    /// </summary>
    public interface IContext : ILifetimeObject, IDisposable
    {
        /// <summary>
        /// 添加一个聚合根实例到当前上下文
        /// </summary>
        void Add(AggregateRoot instance);
        /// <summary>
        /// 根据唯一标识返回一个唯一的聚合根实例
        /// </summary>
        T Load<T>(object id) where T : AggregateRoot;
        /// <summary>
        /// 保存当前上下文范围内产生的所有事件并将事件通过EventPublisher分发出去
        /// </summary>
        void SaveChanges();
    }
}
