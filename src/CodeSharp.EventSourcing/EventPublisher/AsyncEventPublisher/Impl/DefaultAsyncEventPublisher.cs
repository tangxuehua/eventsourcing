//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeSharp.EventSourcing
{
    public class DefaultAsyncEventPublisher : IAsyncEventPublisher
    {
        #region Private Variables

        private ISubscriptionStorage _subscriptionStorage;
        private IMessageTransport _messageTransport;
        private IMessageSerializer _messageSerializer;
        private ILogger _logger;

        #endregion

        #region Constructors

        public DefaultAsyncEventPublisher(ISubscriptionStorage subscriptionStorage, IMessageTransport messageTransport, IMessageSerializer messageSerializer, ILoggerFactory loggerFactory)
        {
            _subscriptionStorage = subscriptionStorage;
            _messageTransport = messageTransport;
            _messageSerializer = messageSerializer;
            _logger = loggerFactory.Create("EventSourcing.DefaultAsyncEventPublisher");
        }

        #endregion

        public void PublishEvent(object evnt)
        {
            var eventTypeName = evnt.GetType().FullName;
            var addresses = _subscriptionStorage.GetSubscriberAddressesForMessage(evnt.GetType());

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
            foreach (var evnt in evnts)
            {
                PublishEvent(evnt);
            }
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
