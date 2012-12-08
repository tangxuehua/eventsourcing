//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;

namespace CodeSharp.EventSourcing.SubscriptionStorage.NHibernate
{
    public class NHibernateSubscriptionStorage : ISubscriptionStorage
    {
        private ISessionFactory _sessionFactory;
        private ILogger _logger;

        public NHibernateSubscriptionStorage(ISessionFactory sessionFactory, ILoggerFactory loggerFactory)
        {
            _sessionFactory = sessionFactory;
            _logger = loggerFactory.Create("EventSourcing.SubscriptionStorage.NHibernate");
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
                    _logger.DebugFormat("Subscriber '{0}' unsubscribes message '{1}'.", subscriberAddress, messageTypeName);
                }
            }
        }
        public IEnumerable<Address> GetSubscriberAddressesForMessage(Type messageType)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var criteria = session.CreateCriteria<Subscription>();
                criteria.Add(Expression.Eq("MessageType", messageType.AssemblyQualifiedName));
                return criteria.List<Subscription>().Select(x => Address.Parse(x.SubscriberAddress));
            }
        }

        public void RefreshSubscriptions()
        {
        }
    }
}
