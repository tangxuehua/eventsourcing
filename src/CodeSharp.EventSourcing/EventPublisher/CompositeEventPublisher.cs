//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// Represents a composite event publisher which contains inner event publishers.
    /// The Publish method will be delegated to all the inner event publishers.
    /// </summary>
    public class CompositeEventPublisher : IEventPublisher
    {
        private IList<IEventPublisher> _innerPublishers;

        public CompositeEventPublisher()
        {
            _innerPublishers = new List<IEventPublisher>();
        }

        public void AddInnerPublisher(IEventPublisher eventPublisher)
        {
            _innerPublishers.Add(eventPublisher);
        }

        public void PublishEvent(object evnt)
        {
            foreach (var eventPublisher in _innerPublishers)
            {
                eventPublisher.PublishEvent(evnt);
            }
        }
        public void PublishEvents(IEnumerable<object> evnts)
        {
            foreach (var eventPublisher in _innerPublishers)
            {
                eventPublisher.PublishEvents(evnts);
            }
        }
    }
}
