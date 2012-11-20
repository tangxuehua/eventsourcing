//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    public interface IAggregateRootTypeProvider
    {
        /// <summary>
        /// 从给定程序集中注册聚合根类型
        /// </summary>
        /// <param name="assemblies"></param>
        void RegisterAggregateRootTypes(params Assembly[] assemblies);
        /// <summary>
        /// 返回所有已注册的聚合根类型
        /// </summary>
        /// <returns></returns>
        IEnumerable<Type> GetAllAggregateRootTypes();
    }
}
