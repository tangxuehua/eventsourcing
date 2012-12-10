//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using CodeSharp.EventSourcing;
using NHibernate;
using NHibernate.Criterion;

namespace CodeSharp.EventSourcing.NHibernate
{
    public class NHibernateSubscriptionStore : ISubscriptionStore
    {
        private ISessionFactory _sessionFactory;
        private InMemorySubscriptionStore _memoryStore;
        private ILogger _logger;

        public NHibernateSubscriptionStore(ISessionFactory sessionFactory, ILoggerFactory loggerFactory)
        {
            _sessionFactory = sessionFactory;
            _memoryStore = new InMemorySubscriptionStore();
            _logger = loggerFactory.Create("EventSourcing.NHibernateSubscriptionStore");
        }

        public void Subscribe(Address address, Type messageType)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var criteria = session.CreateCriteria<Subscription>();
                var subscriberAddress = address.ToString();
                var messageTypeName = messageType.AssemblyQualifiedName;
                criteria.Add(Expression.Eq("SubscriberAddress", subscriberAddress));
                criteria.Add(Expression.Eq("MessageType", messageTypeName));
                var subscription = criteria.UniqueResult<Subscription>();
                if (subscription == null)
                {
                    subscription = new Subscription
                    {
                        UniqueId = Guid.NewGuid().ToString(),
                        SubscriberAddress = subscriberAddress,
                        MessageType = messageTypeName
                    };
                    session.Save(subscription);
                    session.Flush();
                    _memoryStore.Subscribe(address, messageType);
                    _logger.DebugFormat("Subscriber '{0}' subscribes message '{1}'.", subscriberAddress, messageTypeName);
                }
            }
        }
        public void ClearAddressSubscriptions(Address address)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var criteria = session.CreateCriteria<Subscription>();
                var subscriberAddress = address.ToString();
                criteria.Add(Expression.Eq("SubscriberAddress", subscriberAddress));
                var subscriptions = criteria.List<Subscription>();
                foreach (var subscription in subscriptions)
                {
                    session.Delete(subscription);
                }
                session.Flush();
                _memoryStore.ClearAddressSubscriptions(address);
                _logger.DebugFormat("Cleaned up subscriptions of subscriber address '{0}'", subscriberAddress);
            }
        }
        public void Unsubscribe(Address address, Type messageType)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var criteria = session.CreateCriteria<Subscription>();
                var subscriberAddress = address.ToString();
                var messageTypeName = messageType.AssemblyQualifiedName;
                criteria.Add(Expression.Eq("SubscriberAddress", subscriberAddress));
                criteria.Add(Expression.Eq("MessageType", messageTypeName));
                var subscription = criteria.UniqueResult<Subscription>();
                if (subscription != null)
                {
                    session.Delete(subscription);
                    session.Flush();
                    _memoryStore.Unsubscribe(address, messageType);
                    _logger.DebugFormat("Subscriber '{0}' unsubscribes message '{1}'.", subscriberAddress, messageTypeName);
                }
            }
        }
        public IEnumerable<Address> GetSubscriberAddressesForMessage(Type messageType)
        {
            return _memoryStore.GetSubscriberAddressesForMessage(messageType);
        }
        public void RefreshSubscriptions()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var subscriptions = session.CreateCriteria<Subscription>().List<Subscription>();
                var memoryStore = new InMemorySubscriptionStore();

                foreach (var subscription in subscriptions)
                {
                    var address = Address.Parse(subscription.SubscriberAddress as string);
                    var messageType = Type.GetType(subscription.MessageType as string);
                    if (messageType != null)
                    {
                        memoryStore.Subscribe(address, messageType);
                    }
                }

                _memoryStore = memoryStore;
            }
        }
    }
}
