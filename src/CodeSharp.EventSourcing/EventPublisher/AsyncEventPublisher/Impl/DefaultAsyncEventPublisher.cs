//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace CodeSharp.EventSourcing
{
    public class DefaultAsyncEventPublisher : IAsyncEventPublisher
    {
        #region Private Variables

        private ISubscriptionStore _subscriptionStore;
        private IMessageTransport _messageTransport;
        private IMessageSerializer _messageSerializer;
        private Timer _refreshSubscriptionTimer;
        private ILogger _logger;
        private const int RefreshPeriod = 30 * 1000; //默认30秒刷新一次订阅者信息
        private bool _started = false;

        #endregion

        #region Constructors

        public DefaultAsyncEventPublisher(ISubscriptionStore subscriptionStore, IMessageTransport messageTransport, IMessageSerializer messageSerializer, ILoggerFactory loggerFactory)
        {
            _subscriptionStore = subscriptionStore;
            _messageTransport = messageTransport;
            _messageSerializer = messageSerializer;
            _logger = loggerFactory.Create("EventSourcing.DefaultAsyncEventPublisher");
        }

        #endregion

        public void PublishEvent(object evnt)
        {
            if (!_started)
            {
                return;
            }

            var eventTypeName = evnt.GetType().FullName;
            var addresses = _subscriptionStore.GetSubscriberAddressesForMessage(evnt.GetType());

            if (addresses.Count() == 0)
            {
                return;
            }

            var message = CreateMessage(evnt);

            foreach (var address in addresses)
            {
                _logger.DebugFormat("Sending event message '{0}' to address '{1}'.", eventTypeName, address);
                _messageTransport.SendMessage(message, address);
                _logger.DebugFormat("Sent event message '{0}' to address '{1}'.", eventTypeName, address);
            }
        }
        public void PublishEvents(IEnumerable<object> evnts)
        {
            if (!_started)
            {
                return;
            }

            foreach (var evnt in evnts)
            {
                PublishEvent(evnt);
            }
        }
        public void Start()
        {
            _refreshSubscriptionTimer = new Timer((x) => _subscriptionStore.RefreshSubscriptions(), null, 0, RefreshPeriod);
            _started = true;
        }

        private Message CreateMessage(object evnt)
        {
            var result = new Message { MessageIntent = MessageIntentEnum.Publish };
            var stream = new MemoryStream();

            _messageSerializer.Serialize(evnt, stream);

            result.Headers = new Dictionary<string, string>();
            result.Headers.Add(TransportHeaderKeys.MessageFullTypeName, evnt.GetType().AssemblyQualifiedName);
            result.Body = stream.ToArray();
            result.Recoverable = true;
            result.TimeToBeReceived = TimeSpan.MaxValue;

            return result;
        }
    }
}
