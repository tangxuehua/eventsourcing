//Copyright (c) CodeSharp.  All rights reserved.

using FluentNHibernate.Mapping;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace CodeSharp.EventSourcing.NHibernate
{
    public abstract class AggregateRootVersionMap<TAggregateRoot> : ClassMap<AggregateRootVersion<TAggregateRoot>> where TAggregateRoot : AggregateRoot
    {
        public AggregateRootVersionMap(string tableName)
        {
            Table(tableName);
            Id(x => x.AggregateRootId);
            Version(x => x.Version).Generated.Never();
        }
    }
    public class AggregateRootVersionMapping<TAggregateRoot> : ClassMapping<AggregateRootVersion<TAggregateRoot>> where TAggregateRoot : AggregateRoot
    {
        public AggregateRootVersionMapping(string tableName)
        {
            Table(tableName);
            Id(x => x.AggregateRootId);
            Version(x => x.Version, x =>
            {
                x.Generated(VersionGeneration.Never);
                x.UnsavedValue(0);
                x.Type(new Int64Type());
            });
        }
    }
}
