//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 默认的事件订阅者端点实现类
    /// </summary>
    public class DefaultEventSubscriberEndpoint : IEventSubscriberEndpoint
    {
        #region Private Variables

        private bool _started;
        private WorkerThread _messageReceiveWorker;
        private ISubscriptionStorage _subscriptionStorage;
        private IMessageTransport _messageTransport;
        private IMessageSerializer _messageSerializer;
        private IAsyncEventHandlerProvider _asyncEventHandlerProvider;
        private ILogger _logger;

        #endregion

        #region Constructors

        public DefaultEventSubscriberEndpoint() : this(
                ObjectContainer.Resolve<IAsyncEventHandlerProvider>(),
                ObjectContainer.Resolve<ISubscriptionStorage>(),
                ObjectContainer.Resolve<IMessageTransport>(),
                ObjectContainer.Resolve<IMessageSerializer>(),
                ObjectContainer.Resolve<ILoggerFactory>())
        {
        }
        public DefaultEventSubscriberEndpoint(IAsyncEventHandlerProvider asyncEventHandlerProvider, ISubscriptionStorage subscriptionStorage, IMessageTransport messageTransport, IMessageSerializer messageSerializer, ILoggerFactory loggerFactory)
        {
            _asyncEventHandlerProvider = asyncEventHandlerProvider;
            _subscriptionStorage = subscriptionStorage;
            _messageTransport = messageTransport;
            _messageSerializer = messageSerializer;
            _logger = loggerFactory.Create("EventSourcing.DefaultEventSubscriberEndpoint");
            _messageReceiveWorker = new WorkerThread(ReceiveMessage);
            _started = false;
        }

        #endregion

        public void Initialize(string address, bool clearSubscriptions = false)
        {
            var endpointAddress = Address.Parse(address);
            _messageTransport.Init(endpointAddress);
            if (clearSubscriptions)
            {
                _subscriptionStorage.ClearAddressSubscriptions(endpointAddress);
            }
            foreach (var eventType in _asyncEventHandlerProvider.GetAllEventTypes())
            {
                _subscriptionStorage.Subscribe(endpointAddress, eventType);
            }
        }
        public void Start()
        {
            if (!_started)
            {
                _messageReceiveWorker.Start();
                _started = true;
            }
        }
        public void Stop()
        {
            if (_started)
            {
                _messageReceiveWorker.Stop();
            }
        }

        private void ReceiveMessage()
        {
            var message = _messageTransport.Receive();
            if (message != null)
            {
                object evnt = _messageSerializer.Deserialize(message);
                if (evnt != null)
                {
                    _logger.InfoFormat("Received event message, raw event type:{0}, message id:{1}", evnt.GetType().FullName, message.Id);
                    DispatchEventToHandlers(evnt);
                }
            }
        }
        private void DispatchEventToHandlers(object evnt)
        {
            var eventType = evnt.GetType();
            foreach (var metaData in _asyncEventHandlerProvider.GetEventHandlers(eventType))
            {
                try
                {
                    var subscriber = ObjectContainer.Resolve(metaData.SubscriberType);
                    _logger.DebugFormat("Begin to call event subscriber to handle event. Event type:{0}, Subscriber type:{1}", eventType.FullName, subscriber.GetType().FullName);
                    metaData.EventHandler.Invoke(subscriber, new object[] { evnt });
                    _logger.DebugFormat("End to call event subscriber to handle event. Event type:{0}, Subscriber type:{1}", eventType.FullName, subscriber.GetType().FullName);
                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("Error occurred when event subscriber '{0}' handles event '{1}'.", metaData.SubscriberType.FullName, evnt.GetType().FullName), ex);
                }
            }
        }
    }
}
