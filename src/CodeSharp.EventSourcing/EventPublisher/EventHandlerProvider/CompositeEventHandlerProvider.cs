//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 基于组合方式实现的EventHandler元数据提供者，
    /// 当用户希望通过多种方式指定EventHandler元数据时，可以继承此类进行实现。
    /// </summary>
    /// <typeparam name="TMetaData"></typeparam>
    public abstract class CompositeEventHandlerProvider<TMetaData> : IEventHandlerProvider<TMetaData> where TMetaData : class
    {
        private List<IEventHandlerProvider<TMetaData>> providers = new List<IEventHandlerProvider<TMetaData>>();

        public void AddProvider(IEventHandlerProvider<TMetaData> provider)
        {
            providers.Add(provider);
        }

        public void RegisterEventSubscriber(Type subscriberType)
        {
            foreach (var provider in providers)
            {
                provider.RegisterEventSubscriber(subscriberType);
            }
        }
        public void RegisterEventSubscribers(params Assembly[] assemblies)
        {
            foreach (var provider in providers)
            {
                provider.RegisterEventSubscribers(assemblies);
            }
        }
        public bool IsEventSubscriber(Type type)
        {
            return providers.Any(x => x.IsEventSubscriber(type));
        }
        public IEnumerable<Type> GetAllEventTypes()
        {
            var totalEventTypes = new List<Type>();
            providers.ForEach(x => totalEventTypes.AddRange(x.GetAllEventTypes()));
            return totalEventTypes;
        }
        public IEnumerable<TMetaData> GetEventHandlers(Type eventType)
        {
            var totalEventHandlers = new List<TMetaData>();
            providers.ForEach(x => totalEventHandlers.AddRange(x.GetEventHandlers(eventType)));
            return totalEventHandlers;
        }
    }
}
