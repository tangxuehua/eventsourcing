//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// Default implementation of subscription storage, use ADO.NET to persist event subscription.
    /// </summary>
    public class DefaultSubscriptionStorage : ISubscriptionStorage
    {
        private IDbConnectionFactory _connectionFactory;
        private ILogger _logger;

        public DefaultSubscriptionStorage(IDbConnectionFactory connectionFactory, ILoggerFactory loggerFactory)
        {
            _connectionFactory = connectionFactory;
            _logger = loggerFactory.Create("EventSourcing.DefaultSubscriptionStorage");
        }

        public void Subscribe(Address address, Type messageType)
        {
            using (var connection = _connectionFactory.OpenConnection())
            {
                var subscriberAddress = address.ToString();
                var messageTypeName = messageType.AssemblyQualifiedName;
                var table = Configuration.Instance.Properties["subscriptionTable"];
                var count = connection.GetCount(new { SubscriberAddress = subscriberAddress, MessageType = messageTypeName }, table);
                if (count == 0)
                {
                    connection.Insert(new { UniqueId = Guid.NewGuid(), SubscriberAddress = subscriberAddress, MessageType = messageTypeName }, table);
                    _logger.DebugFormat("Subscriber '{0}' subscribes message '{1}'.", subscriberAddress, messageTypeName);
                }
            }
        }
        public void ClearAddressSubscriptions(Address address)
        {
            using (var connection = _connectionFactory.OpenConnection())
            {
                var subscriberAddress = address.ToString();
                var table = Configuration.Instance.Properties["subscriptionTable"];
                connection.Delete(new { SubscriberAddress = subscriberAddress }, table);
                _logger.DebugFormat("Cleaned up subscriptions of subscriber address '{0}'", subscriberAddress);
            }
        }
        public void Unsubscribe(Address address, Type messageType)
        {
            using (var connection = _connectionFactory.OpenConnection())
            {
                var subscriberAddress = address.ToString();
                var messageTypeName = messageType.AssemblyQualifiedName;
                var table = Configuration.Instance.Properties["subscriptionTable"];
                connection.Delete(new { SubscriberAddress = subscriberAddress, MessageType = messageTypeName }, table);
                _logger.DebugFormat("Subscriber '{0}' unsubscribes message '{1}'.", subscriberAddress, messageTypeName);
            }
        }
        public IEnumerable<Address> GetSubscriberAddressesForMessage(Type messageType)
        {
            using (var connection = _connectionFactory.OpenConnection())
            {
                var messageTypeName = messageType.AssemblyQualifiedName;
                var table = Configuration.Instance.Properties["subscriptionTable"];
                var subscriptions = connection.Query(new { MessageType = messageTypeName }, table);
                return subscriptions.Select(x => Address.Parse(x.SubscriberAddress as string));
            }
        }
    }
}
