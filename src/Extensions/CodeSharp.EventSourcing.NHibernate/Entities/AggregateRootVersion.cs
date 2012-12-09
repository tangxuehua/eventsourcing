//Copyright (c) CodeSharp.  All rights reserved.

namespace CodeSharp.EventSourcing.NHibernate
{
    public class AggregateRootVersion
    {
        public virtual string AggregateRootId { get; set; }
        public virtual long Version { get; set; }
    }
    public class AggregateRootVersion<TAggregateRoot> : AggregateRootVersion where TAggregateRoot : AggregateRoot
    {
        public new virtual string AggregateRootId
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
        public new virtual long Version
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
