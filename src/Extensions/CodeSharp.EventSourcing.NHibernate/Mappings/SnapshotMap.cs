//Copyright (c) CodeSharp.  All rights reserved.

using FluentNHibernate.Mapping;
using NHibernate.Mapping.ByCode.Conformist;

namespace CodeSharp.EventSourcing.NHibernate
{
    public abstract class SnapshotMap : ClassMap<Snapshot>
    {
        public SnapshotMap(string tableName)
        {
            Table(tableName);
            Id(x => x.UniqueId);
            Map(x => x.AggregateRootName);
            Map(x => x.AggregateRootId);
            Map(x => x.Version);
            Map(x => x.Name);
            Map(x => x.SerializedData);
            Map(x => x.CreatedTime);
        }
    }
    public class SnapshotMapping : ClassMapping<Snapshot>
    {
        public SnapshotMapping(string tableName)
        {
            Table(tableName);
            Id(x => x.UniqueId);
            Property(x => x.AggregateRootName);
            Property(x => x.AggregateRootId);
            Property(x => x.Version);
            Property(x => x.Name);
            Property(x => x.SerializedData);
            Property(x => x.CreatedTime);
        }
    }
}
