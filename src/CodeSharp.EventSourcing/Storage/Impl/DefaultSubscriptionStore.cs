//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// Default implementation of subscription store, use ADO.NET to persist event subscription.
    /// </summary>
    public class DefaultSubscriptionStore : ISubscriptionStore
    {
        private IDbConnectionFactory _connectionFactory;
        private InMemorySubscriptionStore _memoryStore;
        private ILogger _logger;

        public DefaultSubscriptionStore(IDbConnectionFactory connectionFactory, ILoggerFactory loggerFactory)
        {
            _connectionFactory = connectionFactory;
            _memoryStore = new InMemorySubscriptionStore();
            _logger = loggerFactory.Create("EventSourcing.DefaultSubscriptionStore");
        }

        public void Subscribe(Address address, Type messageType)
        {
            using (var connection = _connectionFactory.OpenConnection())
            {
                var subscriberAddress = address.ToString();
                var messageTypeName = messageType.AssemblyQualifiedName;
                var table = Configuration.Instance.GetSetting<string>("subscriptionTable");
                var count = connection.GetCount(new { SubscriberAddress = subscriberAddress, MessageType = messageTypeName }, table);
                if (count == 0)
                {
                    connection.Insert(new { UniqueId = Guid.NewGuid(), SubscriberAddress = subscriberAddress, MessageType = messageTypeName }, table);
                    _memoryStore.Subscribe(address, messageType);
                    _logger.DebugFormat("Subscriber '{0}' subscribes message '{1}'.", subscriberAddress, messageTypeName);
                }
            }
        }
        public void ClearAddressSubscriptions(Address address)
        {
            using (var connection = _connectionFactory.OpenConnection())
            {
                var subscriberAddress = address.ToString();
                var table = Configuration.Instance.GetSetting<string>("subscriptionTable");
                connection.Delete(new { SubscriberAddress = subscriberAddress }, table);
                _memoryStore.ClearAddressSubscriptions(address);
                _logger.DebugFormat("Cleaned up subscriptions of subscriber address '{0}'", subscriberAddress);
            }
        }
        public void Unsubscribe(Address address, Type messageType)
        {
            using (var connection = _connectionFactory.OpenConnection())
            {
                var subscriberAddress = address.ToString();
                var messageTypeName = messageType.AssemblyQualifiedName;
                var table = Configuration.Instance.GetSetting<string>("subscriptionTable");
                connection.Delete(new { SubscriberAddress = subscriberAddress, MessageType = messageTypeName }, table);
                _memoryStore.Unsubscribe(address, messageType);
                _logger.DebugFormat("Subscriber '{0}' unsubscribes message '{1}'.", subscriberAddress, messageTypeName);
            }
        }
        public IEnumerable<Address> GetSubscriberAddressesForMessage(Type messageType)
        {
            return _memoryStore.GetSubscriberAddressesForMessage(messageType);
        }
        public void RefreshSubscriptions()
        {
            using (var connection = _connectionFactory.OpenConnection())
            {
                var table = Configuration.Instance.GetSetting<string>("subscriptionTable");
                var subscriptions = connection.QueryAll(table);
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
