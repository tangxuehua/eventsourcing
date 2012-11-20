//Copyright (c) CodeSharp.  All rights reserved.

namespace CodeSharp.EventSourcing.EventStore.NHibernate
{
    public class AggregateRootVersion
    {
        public string AggregateRootId { get; set; }
        public long Version { get; set; }
    }
    public class AggregateRootVersion<TAggregateRoot> : AggregateRootVersion where TAggregateRoot : AggregateRoot
    {
        public new string AggregateRootId
        {
            get
            {
                return base.AggregateRootId;
            }
            set
            {
                base.AggregateRootId = value;
            }
        }
        public new long Version
        {
            get
            {
                return base.Version;
            }
            set
            {
                base.Version = value;
            }
        }
    }
}
