//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 默认的快照类型提供者
    /// </summary>
    public class DefaultSnapshotTypeProvider : ISnapshotTypeProvider
    {
        private IList<Type> _snapshotTypes = new List<Type>();

        /// <summary>
        /// 设置该属性允许用户使用自己的接口来标记一个类为一个快照类
        /// </summary>
        public Type SnapshotInterfaceType { get; set; }
        /// <summary>
        /// 设置改属性允许用户使用自己的特性来标记一个类为一个快照类
        /// </summary>
        public Type SnapshotAttributeType { get; set; }

        /// <summary>
        /// 添加一个快照类
        /// </summary>
        /// <param name="snapshotType"></param>
        public void AddSnspshotType(Type snapshotType)
        {
            if (!_snapshotTypes.Contains(snapshotType))
            {
                _snapshotTypes.Add(snapshotType);
            }
        }

        public bool IsSnapshot(Type type)
        {
            return CheckType(_snapshotTypes, SnapshotInterfaceType, type, SnapshotAttributeType, typeof(SnapshotAttribute));
        }

        private bool CheckType(IEnumerable<Type> totalTypes, Type typeInterface, Type typeToCheck, Type customizeSnapshotAttributeType, Type defaultSnapshotAttributeType)
        {
            return (typeInterface != null && typeInterface.IsAssignableFrom(typeToCheck))
                || (totalTypes != null && totalTypes.Any(x => x == typeToCheck))
                || (customizeSnapshotAttributeType != null ? IsTypeHasAttribute(typeToCheck, customizeSnapshotAttributeType) : false)
                || IsTypeHasAttribute(typeToCheck, defaultSnapshotAttributeType);
        }
        private bool IsTypeHasAttribute(Type type, Type attribute)
        {
            return type.IsClass
                && !type.IsAbstract
                && !type.IsGenericType
                && !type.IsGenericTypeDefinition
                && type.GetCustomAttributes(attribute, false).Count() > 0;
        }
    }
}
