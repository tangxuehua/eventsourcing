//Copyright (c) CodeSharp.  All rights reserved.

namespace CodeSharp.EventSourcing.NHibernate
{
    public class Subscription
    {
        public virtual string UniqueId { get; set; }
        public virtual string SubscriberAddress { get; set; }
        public virtual string MessageType { get; set; }
    }
}
