//Copyright (c) CodeSharp.  All rights reserved.

namespace CodeSharp.EventSourcing.SubscriptionStorage.NHibernate
{
    public class Subscription
    {
        public string UniqueId { get; set; }
        public string SubscriberAddress { get; set; }
        public string MessageType { get; set; }
    }
}
