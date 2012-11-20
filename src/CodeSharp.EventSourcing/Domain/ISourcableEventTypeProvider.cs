//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    public interface ISourcableEventTypeProvider
    {
        /// <summary>
        /// 扫描给定程序集中所有的可溯源事件，将可溯源事件的类型与其对应的聚合根的类型建立映射关系
        /// </summary>
        /// <param name="assemblies"></param>
        void RegisterMappings(params Assembly[] assemblies);
        /// <summary>
        /// 返回聚合根对应的可溯源事件的类型
        /// </summary>
        Type GetSourcableEventType(Type aggregateRootType);
    }
}
