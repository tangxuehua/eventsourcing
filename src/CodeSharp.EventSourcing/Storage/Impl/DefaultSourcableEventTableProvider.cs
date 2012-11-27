//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;

namespace CodeSharp.EventSourcing
{
    public class DefaultSourcableEventTableProvider : ISourcableEventTableProvider
    {
        public void AddMapping(Type aggregateRootType, string table)
        {
            throw new NotImplementedException();
        }

        public string GetTable(Type aggregateRootType)
        {
            return Configuration.Instance.Properties["eventTable"];
        }
    }
}
