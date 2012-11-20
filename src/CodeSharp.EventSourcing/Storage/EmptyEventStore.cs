//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// Default empty implementation of event store, not do any event storage.
    /// </summary>
    public class EmptyEventStore : IEventStore
    {
        public void StoreEvents(IEnumerable<SourcableEvent> evnts)
        {
        }
        public IEnumerable<SourcableEvent> GetEvents(string aggregateRootId, Type aggregateRootType, long minVersion, long maxVersion)
        {
            return new List<SourcableEvent>();
        }
    }
}
