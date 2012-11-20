//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 内部辅助类，用于通知聚合根响应事件
    /// </summary>
    internal class SourcableEventAggregateHandlerCaller
    {
        public static void CallEventHandler(Type aggregateRootType, MethodInfo eventHandler, object evnt, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            var eventType = evnt.GetType();
            var propertyInfo = eventType.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new EventSourcingException(string.Format("Property '{0}' not exist in event type '{1}'", propertyName, eventType.FullName));
            }

            var aggregateRootId = propertyInfo.GetValue(evnt, null);
            if (aggregateRootId != null)
            {
                var logger = ObjectContainer.Resolve<ILoggerFactory>().Create("EventSourcing.SourcableEventAggregateHandlerCaller");
                var aggregateRoot = AggregateRootLocator.GetAggregateRoot(aggregateRootType, aggregateRootId);
                if (aggregateRoot != null)
                {
                    logger.DebugFormat("Invoking agregate root event handler. aggregate root type:{0} event type:{1}.", aggregateRootType.Name, eventType.Name);
                    eventHandler.Invoke(aggregateRoot, new object[] { evnt });
                    logger.DebugFormat("Invoked agregate root event handler. aggregate root type:{0} event type:{1}.", aggregateRootType.Name, eventType.Name);
                }
                else
                {
                    logger.ErrorFormat(
                        "Could not find the aggregate root as a subscriber of event, Please verify whether it was deleted. AggregateRoot Id:{0}, Event Type:{1}, AggregateRootId PropertyName:{2}",
                        aggregateRootId,
                        eventType.FullName,
                        propertyName
                    );
                }
            }
        }
        public static void CallEventHandler(MethodInfo eventHandler, object evnt, IEnumerable<Path> paths)
        {
            if (paths == null || paths.Count() == 0) return;

            var logger = ObjectContainer.Resolve<ILoggerFactory>().Create("EventSourcing.SourcableEventAggregateHandlerCaller");
            var eventType = evnt.GetType();
            var sourceObject = evnt;
            AggregateRoot aggregateRoot = null;

            for(var index = 0; index < paths.Count(); index++)
            {
                var path = paths.ElementAt(index);
                if (index == paths.Count() - 1)
                {
                    aggregateRoot = GetAggregateRootFromProperty(path.AggregateRootType, sourceObject, path.PropertyName);
                }
                else
                {
                    aggregateRoot = GetAggregateRootFromProperty(path.AggregateRootType, sourceObject, path.PropertyName);
                }

                if (aggregateRoot == null)
                {
                    break;
                }
                else
                {
                    sourceObject = aggregateRoot;
                }
            }

            if (aggregateRoot != null)
            {
                logger.DebugFormat("Invoking agregate root event handler. aggregate root type:{0} event type:{1}.", aggregateRoot.GetType().Name, eventType.Name);
                eventHandler.Invoke(aggregateRoot, new object[] { evnt });
                logger.DebugFormat("Invoked agregate root event handler. aggregate root type:{0} event type:{1}.", aggregateRoot.GetType().Name, eventType.Name);
            }
            else
            {
                logger.ErrorFormat(
                    "Could not find the aggregate root as a subscriber of event, Please verify whether it was deleted. Event Type:{0}, Paths:{1}",
                    eventType.FullName,
                    string.Join("->", paths.Select(x => string.Format("Type:{0},PropertyName:{1}", x.AggregateRootType.Name, x.PropertyName)).ToArray())
                );
            }
        }

        private static AggregateRoot GetAggregateRootFromProperty(Type aggregateRootType, object sourceObject, string propertyName)
        {
            if (aggregateRootType == null || sourceObject == null || string.IsNullOrEmpty(propertyName)) return null;

            var propertyInfo = sourceObject.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new EventSourcingException(string.Format("Property '{0}' not exist in object type '{1}'", propertyName, sourceObject.GetType().FullName));
            }
            var aggregateRootId = propertyInfo.GetValue(sourceObject, null);
            if (aggregateRootId != null)
            {
                return AggregateRootLocator.GetAggregateRoot(aggregateRootType, aggregateRootId);
            }

            return null;
        }
    }
}
