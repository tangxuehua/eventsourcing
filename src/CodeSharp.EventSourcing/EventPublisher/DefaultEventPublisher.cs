//Copyright (c) CodeSharp.  All rights reserved.

using System.Collections.Generic;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 默认的事件分发器
    /// </summary>
    public class DefaultEventPublisher : IEventPublisher
    {
        private CompositeEventPublisher _compositeEventPublisher = new CompositeEventPublisher();

        public DefaultEventPublisher()
        {
            if (!ObjectContainer.IsRegistered(typeof(SyncEventPublisher)))
            {
                ObjectContainer.Register<SyncEventPublisher, SyncEventPublisher>(LifeStyle.Transient);
            }
            if (!ObjectContainer.IsRegistered(typeof(AsyncEventPublisher)))
            {
                ObjectContainer.Register<AsyncEventPublisher, AsyncEventPublisher>(LifeStyle.Transient);
            }

            var syncEventPublisher = ObjectContainer.Resolve<SyncEventPublisher>();
            var asyncEventPublisher = ObjectContainer.Resolve<AsyncEventPublisher>();

            _compositeEventPublisher.AddInnerPublisher(syncEventPublisher);
            _compositeEventPublisher.AddInnerPublisher(asyncEventPublisher);
        }

        public void PublishEvent(object evnt)
        {
            _compositeEventPublisher.PublishEvent(evnt);
        }
        public void PublishEvents(IEnumerable<object> evnts)
        {
            _compositeEventPublisher.PublishEvents(evnts);
        }
    }
}
