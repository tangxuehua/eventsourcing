//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 定义提供聚合根中发生的可溯源事件的内部回调函数的接口
    /// </summary>
    public interface IAggregateRootInternalHandlerProvider
    {
        /// <summary>
        /// 注册指定聚合根的指定可溯源事件的回调函数
        /// </summary>
        /// <param name="aggregateRootType"></param>
        /// <param name="eventType"></param>
        /// <param name="eventHandler"></param>
        void RegisterInternalHandler(Type aggregateRootType, Type eventType, MethodInfo eventHandler);
        /// <summary>
        /// 从给定程序集中注册所有符合条件的回调函数
        /// </summary>
        /// <param name="assemblies"></param>
        void RegisterInternalHandlers(params Assembly[] assemblies);
        /// <summary>
        /// 获取聚合根可溯源事件的内部响应函数
        /// </summary>
        Action<AggregateRoot, object> GetInternalEventHandler(Type aggregateRootType, Type eventType);
    }
}
