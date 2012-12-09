//Copyright (c) CodeSharp.  All rights reserved.

using FluentNHibernate.Mapping;
using NHibernate.Mapping.ByCode.Conformist;

namespace CodeSharp.EventSourcing.NHibernate
{
    public abstract class SourcableEventMap<TAggregateRoot> : ClassMap<SourcableEvent<TAggregateRoot>> where TAggregateRoot : AggregateRoot
    {
        public SourcableEventMap(string tableName)
        {
            Table(tableName);
            Id(x => x.UniqueId);
            Map(x => x.AggregateRootName);
            Map(x => x.AggregateRootId);
            Map(x => x.Version);
            Map(x => x.Name);
            Map(x => x.Data);
            Map(x => x.OccurredTime);
        }
    }
    public class SourcableEventMapping<TAggregateRoot> : ClassMapping<SourcableEvent<TAggregateRoot>> where TAggregateRoot : AggregateRoot
    {
        public SourcableEventMapping(string tableName)
        {
            Table(tableName);
            Id(x => x.UniqueId);
            Property(x => x.AggregateRootName);
            Property(x => x.AggregateRootId);
            Property(x => x.Version);
            Property(x => x.Name);
            Property(x => x.Data);
            Property(x => x.OccurredTime);
        }
    }
}
