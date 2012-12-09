//Copyright (c) CodeSharp.  All rights reserved.

using NHibernate.Mapping.ByCode.Conformist;

namespace CodeSharp.EventSourcing.NHibernate
{
    public class SubscriptionMapping : ClassMapping<Subscription>
    {
        public SubscriptionMapping(string tableName)
        {
            Table(tableName);
            Id(x => x.UniqueId);
            Property(x => x.SubscriberAddress);
            Property(x => x.MessageType);
        }
    }
}
