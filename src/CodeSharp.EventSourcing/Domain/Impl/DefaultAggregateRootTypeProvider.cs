//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 默认的聚合根类型提供者
    /// </summary>
    public class DefaultAggregateRootTypeProvider : IAggregateRootTypeProvider
    {
        private List<Type> _aggregateRootTypeList = new List<Type>();

        public void RegisterAggregateRootTypes(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                _aggregateRootTypeList.AddRange(assembly.GetTypes().Where(t => TypeUtils.IsAggregateRoot(t)));
            }
        }
        public IEnumerable<Type> GetAllAggregateRootTypes()
        {
            return _aggregateRootTypeList.AsReadOnly();
        }
    }
}
