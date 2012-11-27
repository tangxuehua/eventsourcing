//Copyright (c) CodeSharp.  All rights reserved.

using System.Collections.Generic;

namespace CodeSharp.EventSourcing
{
    public class DefaultSyncEventPublisher : ISyncEventPublisher
    {
        private ISyncEventHandlerProvider _syncEventHandlerProvider;
        private ILogger _logger;

        public DefaultSyncEventPublisher(ISyncEventHandlerProvider syncEventHandlerProvider, ILoggerFactory loggerFactory)
        {
            _syncEventHandlerProvider = syncEventHandlerProvider;
            _logger = loggerFactory.Create("EventSourcing.DefaultSyncEventPublisher");
        }

        public void PublishEvent(object evnt)
        {
            var eventType = evnt.GetType();
            foreach (var metaData in _syncEventHandlerProvider.GetEventHandlers(eventType))
            {
                var subscriber = ObjectContainer.Resolve(metaData.SubscriberType);
                _logger.DebugFormat("Begin to call event subscriber to handle event. Event type:{0}, Subscriber type:{1}", eventType.FullName, subscriber.GetType().FullName);
                metaData.EventHandler.Invoke(subscriber, new object[] { evnt });
                _logger.DebugFormat("End to call event subscriber to handle event. Event type:{0}, Subscriber type:{1}", eventType.FullName, subscriber.GetType().FullName);
            }
        }
        public void PublishEvents(IEnumerable<object> evnts)
        {
            foreach (var evnt in evnts)
            {
                PublishEvent(evnt);
            }
        }
    }
}
