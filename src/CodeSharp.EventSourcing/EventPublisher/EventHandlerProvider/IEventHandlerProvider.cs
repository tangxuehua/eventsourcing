//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    public interface IEventHandlerProvider<TMetaData> where TMetaData : class
    {
        void RegisterEventSubscriber(Type subscriberType);
        void RegisterEventSubscribers(params Assembly[] assemblies);
        bool IsEventSubscriber(Type type);
        IEnumerable<Type> GetAllEventTypes();
        IEnumerable<TMetaData> GetEventHandlers(Type eventType);
    }
}
