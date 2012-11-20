//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 默认的聚合根可溯源事件内部处理函数元数据提供者
    /// </summary>
    public class DefaultAggregateRootInternalHandlerProvider : IAggregateRootInternalHandlerProvider
    {
        private IDictionary<Type, IDictionary<Type, MethodInfo>> _mappings = new Dictionary<Type, IDictionary<Type, MethodInfo>>();

        public void RegisterInternalHandler(Type aggregateRootType, Type eventType, MethodInfo eventHandler)
        {
            IDictionary<Type, MethodInfo> eventHandlerDic;

            if (!_mappings.TryGetValue(aggregateRootType, out eventHandlerDic))
            {
                eventHandlerDic = new Dictionary<Type, MethodInfo>();
                _mappings.Add(aggregateRootType, eventHandlerDic);
            }

            if (eventHandlerDic.ContainsKey(eventType))
            {
                throw new EventSourcingException(string.Format("聚合根（{0}）上定义了重复的事件（{1}）响应函数。", aggregateRootType.FullName, eventType.FullName));
            }

            eventHandlerDic.Add(eventType, eventHandler);
        }
        public void RegisterInternalHandlers(params Assembly[] assemblies)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (var assembly in assemblies)
            {
                foreach (var aggregateRootType in assembly.GetTypes().Where(t => TypeUtils.IsAggregateRoot(t)))
                {
                    var entries = from method in aggregateRootType.GetMethods(bindingFlags)
                                  let parameters = method.GetParameters()
                                  where (method.Name == "Handle" || method.Name.StartsWith("On")) && parameters.Length == 1
                                  select new { Method = method, EventType = parameters.First().ParameterType };

                    foreach (var entry in entries)
                    {
                        RegisterInternalHandler(aggregateRootType, entry.EventType, entry.Method);
                    }
                }
            }
        }
        public Action<AggregateRoot, object> GetInternalEventHandler(Type aggregateRootType, Type eventType)
        {
            IDictionary<Type, MethodInfo> eventHandlerDic;
            MethodInfo eventHandler;

            if (_mappings.TryGetValue(aggregateRootType, out eventHandlerDic))
            {
                if (eventHandlerDic.TryGetValue(eventType, out eventHandler))
                {
                    return new Action<AggregateRoot, object>((aggregateRoot, evnt) => eventHandler.Invoke(aggregateRoot, new object[] { evnt }));
                }
            }

            return null;
        }
    }
}
