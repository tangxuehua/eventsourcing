//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;

namespace CodeSharp.EventSourcing
{
    public class DefaultAggregateRootVersionTableProvider : IAggregateRootVersionTableProvider
    {
        private IDictionary<Type, string> _mappings = new Dictionary<Type, string>();

        public void AddMapping(Type aggregateRootType, string table)
        {
            if (!_mappings.ContainsKey(aggregateRootType))
            {
                _mappings.Add(aggregateRootType, table);
            }
        }

        public string GetTable(Type aggregateRootType)
        {
            if (_mappings.ContainsKey(aggregateRootType))
            {
                return _mappings[aggregateRootType];
            }
            return Configuration.Instance.GetSetting<string>("versionTable");
        }
    }
}
