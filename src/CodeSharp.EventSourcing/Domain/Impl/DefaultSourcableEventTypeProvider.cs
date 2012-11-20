//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 默认的可溯源事件类型提供者
    /// </summary>
    public class DefaultSourcableEventTypeProvider : ISourcableEventTypeProvider
    {
        private readonly Dictionary<Type, Type> _mappings = new Dictionary<Type, Type>();

        public void RegisterMappings(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var sourcableEventType in assembly.GetTypes().Where(x => TypeUtils.IsSourcableEvent(x)))
                {
                    RegisterMapping(GetAggregateRootType(sourcableEventType), sourcableEventType);
                }
            }
        }
        public Type GetSourcableEventType(Type aggregateRootType)
        {
            if (_mappings.ContainsKey(aggregateRootType))
            {
                return _mappings[aggregateRootType];
            }
            return typeof(SourcableEvent<>).MakeGenericType(aggregateRootType);
        }

        private void RegisterMapping(Type aggregateRootType, Type sourcableEventType)
        {
            Utils.AssertTypeInheritance(aggregateRootType, typeof(AggregateRoot));
            Utils.AssertTypeInheritance(sourcableEventType, typeof(SourcableEvent));

            if (_mappings.ContainsKey(aggregateRootType))
            {
                throw new EventSourcingException(string.Format("不能为同一个类型的聚合根（Type:{0}）注册两次SourcableEvent的Mapping信息", aggregateRootType.FullName));
            }

            _mappings.Add(aggregateRootType, sourcableEventType);
        }
        private Type GetAggregateRootType(Type sourcableEventType)
        {
            return sourcableEventType
                    .GetInterfaces()
                    .Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISourcableEvent<>))
                    .GetGenericArguments()[0];
        }
    }
}
