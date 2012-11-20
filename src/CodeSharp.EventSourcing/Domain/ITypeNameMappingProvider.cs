//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 一个接口用于维护某个类型与其名称之间的一一对应关系
    /// </summary>
    public interface ITypeNameMappingProvider
    {
        /// <summary>
        /// 为一个类型注册一个名称，名称和类型一一对应
        /// </summary>
        void RegisterMapping(NameTypeMappingType mappingType, string name, Type type);
        /// <summary>
        /// 从给定程序集中扫描所有类型与其名称的映射关系
        /// </summary>
        void RegisterMappings(NameTypeMappingType mappingType, params Assembly[] assemblies);
        /// <summary>
        /// 根据类型名称返回类型
        /// </summary>
        Type GetType(NameTypeMappingType mappingType, string name);
        /// <summary>
        /// 根据类型返回类型名称
        /// </summary>
        string GetName(NameTypeMappingType mappingType, Type type);
        /// <summary>
        /// 返回给定的类型是否存在一个对应的名称与之对应
        /// </summary>
        bool IsTypeExist(NameTypeMappingType mappingType, Type type);
        /// <summary>
        /// 返回给定的名称是否存在一个对应的类型与之对应
        /// </summary>
        bool IsNameExist(NameTypeMappingType mappingType, string name);
    }
    ///<summary>
    /// 表示类型与名称之间映射关系的种类
    ///</summary>
    public enum NameTypeMappingType
    {
        /// <summary>
        /// 聚合根的类型与其名称之间的映射
        /// </summary>
        AggregateRootMapping,
        /// <summary>
        /// 可溯源事件的类型与其名称之间的映射
        /// </summary>
        SourcableEventMapping,
        /// <summary>
        /// 快照的类型与其名称之间的映射
        /// </summary>
        SnapshotMapping
    }
}
