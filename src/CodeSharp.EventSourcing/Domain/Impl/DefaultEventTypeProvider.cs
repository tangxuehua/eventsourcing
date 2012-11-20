//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 默认的事件类型提供者
    /// </summary>
    public class DefaultEventTypeProvider : IEventTypeProvider
    {
        private IList<Type> _sourcableEventTypes = new List<Type>();
        private IList<Type> _notityEventTypes = new List<Type>();

        /// <summary>
        /// 设置该属性允许用户使用自己的接口来标记一个事件为可溯源事件
        /// </summary>
        public Type SourcableEventInterfaceType { get; set; }
        /// <summary>
        /// 设置改属性允许用户使用自己的特性来标记一个事件为可溯源事件
        /// </summary>
        public Type SourcableEventAttributeType { get; set; }
        /// <summary>
        /// 设置该属性允许用户使用自己的接口来标记一个事件为通知事件
        /// </summary>
        public Type NotifyEventInterfaceType { get; set; }
        /// <summary>
        /// 设置改属性允许用户使用自己的特性来标记一个事件为通知事件
        /// </summary>
        public Type NotifyEventAttributeType { get; set; }

        public void AddSourcableEventType(Type eventType)
        {
            if (!_sourcableEventTypes.Contains(eventType))
            {
                _sourcableEventTypes.Add(eventType);
            }
        }
        public void AddNotifyEventType(Type eventType)
        {
            if (!_notityEventTypes.Contains(eventType))
            {
                _notityEventTypes.Add(eventType);
            }
        }
        public bool IsSourcableEvent(Type eventType)
        {
            return CheckEventType(_sourcableEventTypes, SourcableEventInterfaceType, eventType, SourcableEventAttributeType, typeof(SourcableEventAttribute));
        }
        public bool IsNotityEvent(Type eventType)
        {
            return CheckEventType(_notityEventTypes, NotifyEventInterfaceType, eventType, NotifyEventAttributeType, typeof(NotifyEventAttribute));
        }

        private bool CheckEventType(IEnumerable<Type> totalTypes, Type typeInterface, Type typeToCheck, Type customizeEventAttributeType, Type defaultEventAttributeType)
        {
            return (typeInterface != null && typeInterface.IsAssignableFrom(typeToCheck))
                || (totalTypes != null && totalTypes.Any(x => x == typeToCheck))
                || (customizeEventAttributeType != null ? IsEventTypeHasAttribute(typeToCheck, customizeEventAttributeType) : false)
                || IsEventTypeHasAttribute(typeToCheck, defaultEventAttributeType);
        }
        private bool IsEventTypeHasAttribute(Type type, Type eventAttribute)
        {
            return type.IsClass
                && !type.IsAbstract
                && !type.IsGenericType
                && !type.IsGenericTypeDefinition
                && type.GetCustomAttributes(eventAttribute, false).Count() > 0;
        }
    }
}
