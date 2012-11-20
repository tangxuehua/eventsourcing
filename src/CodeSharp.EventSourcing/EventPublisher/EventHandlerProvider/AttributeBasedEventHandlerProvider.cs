//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 基于特性实现的EventHandler元数据提供者
    /// </summary>
    /// <typeparam name="TEventHandlerAttribute">特性类型</typeparam>
    /// <typeparam name="TEventHandlerMetaData">元数据类型</typeparam>
    public abstract class AttributeBasedEventHandlerProvider<TEventHandlerAttribute, TEventHandlerMetaData> : IEventHandlerProvider<TEventHandlerMetaData>
        where TEventHandlerAttribute : Attribute
        where TEventHandlerMetaData : class
    {
        protected readonly Dictionary<Type, List<TEventHandlerMetaData>> _eventHandlerDictionary = new Dictionary<Type, List<TEventHandlerMetaData>>();

        public virtual void RegisterEventSubscribers(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var subscriberType in assembly.GetTypes().Where(x => IsEventSubscriber(x)))
                {
                    RegisterEventSubscriber(subscriberType);
                }
            }
        }
        public virtual void RegisterEventSubscriber(Type subscriberType)
        {
            foreach (var eventHandler in ScanEventHandlers(subscriberType))
            {
                RegisterEventHandler(GetEventType(eventHandler), CreateMetaData(subscriberType, eventHandler));
            }
        }
        public bool IsEventSubscriber(Type type)
        {
            return type != null && type.IsClass && !type.IsAbstract && ScanEventHandlers(type).Count() > 0;
        }
        public virtual IEnumerable<TEventHandlerMetaData> GetEventHandlers(Type eventType)
        {
            var eventHandlers = new List<TEventHandlerMetaData>();
            foreach (var key in _eventHandlerDictionary.Keys)
            {
                if (key.IsAssignableFrom(eventType))
                {
                    eventHandlers.AddRange(_eventHandlerDictionary[key]);
                }
            }
            return eventHandlers;
        }
        public IEnumerable<Type> GetAllEventTypes()
        {
            return _eventHandlerDictionary.Keys.ToList();
        }

        protected virtual bool IsEventHandler(MethodInfo method)
        {
            return method.GetCustomAttributes(typeof(TEventHandlerAttribute), false).Count() == 1;
        }
        protected abstract TEventHandlerMetaData CreateMetaData(Type subscriberType, MethodInfo eventHandler);
        protected virtual void RegisterEventHandler(Type eventType, TEventHandlerMetaData metaData)
        {
            List<TEventHandlerMetaData> metaDataList = null;
            if (!_eventHandlerDictionary.TryGetValue(eventType, out metaDataList))
            {
                metaDataList = new List<TEventHandlerMetaData>();
                _eventHandlerDictionary.Add(eventType, metaDataList);
            }
            metaDataList.Add(metaData);
        }
        protected virtual IEnumerable<MethodInfo> ScanEventHandlers(Type subscriberType)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            return subscriberType.GetMethods(flags).Where(x => IsEventHandler(x));
        }
        protected virtual Type GetEventType(MethodInfo eventHandler)
        {
            return eventHandler.GetParameters().First().ParameterType;
        }
    }
}
