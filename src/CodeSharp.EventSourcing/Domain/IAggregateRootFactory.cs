//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 表示一个创建一个空的聚合根的接口
    /// </summary>
    public interface IAggregateRootFactory
    {
        /// <summary>
        /// 创建给定类型的一个空的聚合根实例
        /// </summary>
        AggregateRoot CreateAggregateRoot(Type aggregateRootType);
    }
}
