//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 基于接口实现的EventHandler元数据提供者
    /// </summary>
    /// <typeparam name="TEventHandlerMetaData">元数据类型</typeparam>
    public abstract class InterfaceBasedEventHandlerProvider<TEventHandlerMetaData> : IEventHandlerProvider<TEventHandlerMetaData>
        where TEventHandlerMetaData : class
    {
        protected readonly Dictionary<Type, List<TEventHandlerMetaData>> _eventHandlerDictionary = new Dictionary<Type, List<TEventHandlerMetaData>>();
        protected abstract Type EventSubscriberInterfaceType { get; }

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
            foreach (var eventSubscriberInterface in ScanEventSubscriberInterfaces(subscriberType))
            {
                RegisterEventHandler(GetEventType(eventSubscriberInterface), CreateMetaData(subscriberType, GetEventHandler(subscriberType, eventSubscriberInterface)));
            }
        }
        public bool IsEventSubscriber(Type type)
        {
            return type != null && type.IsClass && !type.IsAbstract && ScanEventSubscriberInterfaces(type).Count() > 0;
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
        protected virtual IEnumerable<Type> ScanEventSubscriberInterfaces(Type subscriberType)
        {
            return subscriberType.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == EventSubscriberInterfaceType);
        }
        protected virtual Type GetEventType(Type eventSubscriberInterface)
        {
            return eventSubscriberInterface.GetGenericArguments().First();
        }
        protected virtual MethodInfo GetEventHandler(Type subscriberType, Type eventSubscriberInterface)
        {
            return subscriberType.GetInterfaceMap(eventSubscriberInterface).InterfaceMethods.First();
        }
    }
}
