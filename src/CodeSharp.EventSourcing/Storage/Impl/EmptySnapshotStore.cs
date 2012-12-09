//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// Default empty implementation of snapshot store, not do any snapshot store.
    /// </summary>
    public class EmptySnapshotStore : ISnapshotStore
    {
        public void StoreShapshot(Snapshot snapshot)
        {
        }
        public Snapshot GetLastestSnapshot(string aggregateRootId, Type aggregateRootType)
        {
            return null;
        }
    }
}
