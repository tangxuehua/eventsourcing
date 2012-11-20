//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Linq;

namespace CodeSharp.EventSourcing
{
    public sealed class TypeUtils
    {
        /// <summary>
        /// 判断给定类型是否是MVC Controller
        /// </summary>
        public static bool IsController(Type type)
        {
            return type != null
                   && type.Name.EndsWith("Controller", StringComparison.InvariantCultureIgnoreCase)
                   && !type.IsAbstract;
        }
        /// <summary>
        /// 判断给定类型是否是仓储
        /// </summary>
        public static bool IsRepository(Type type)
        {
            return type != null
                 && type.Name.EndsWith("Repository", StringComparison.InvariantCultureIgnoreCase)
                 && !type.IsAbstract
                 && !type.IsInterface;
        }
        /// <summary>
        /// 判断给定类型是否是服务
        /// </summary>
        public static bool IsService(Type type)
        {
            return type != null
                 && type.Name.EndsWith("Service", StringComparison.InvariantCultureIgnoreCase)
                 && !type.IsAbstract
                 && !type.IsInterface;
        }
        /// <summary>
        /// 判断给定类型是否是组件，标记了ComponentAttribute特性的类
        /// </summary>
        public static bool IsComponent(Type type)
        {
            return type != null
                 && type.GetCustomAttributes(typeof(ComponentAttribute), false).Count() > 0
                 && !type.IsAbstract
                 && !type.IsInterface;
        }
        /// <summary>
        /// 判断给定类型是否是事件订阅者
        /// </summary>
        public static bool IsEventSubscriber(Type type)
        {
            return type != null
                 && type.Name.EndsWith("EventSubscriber", StringComparison.InvariantCultureIgnoreCase)
                 && !type.IsAbstract
                 && !type.IsInterface;
        }
        /// <summary>
        /// 判断给定类型是否是聚合根
        /// </summary>
        public static bool IsAggregateRoot(Type type)
        {
            return type.IsClass
                && !type.IsAbstract
                && typeof(AggregateRoot).IsAssignableFrom(type);
        }
        /// <summary>
        /// 判断给定类型是否是可溯源事件
        /// </summary>
        public static bool IsSourcableEvent(Type type)
        {
            return type != null
                && type.IsClass
                && type.Name.EndsWith("SourcableEvent", StringComparison.OrdinalIgnoreCase)
                && !type.IsAbstract
                && type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISourcableEvent<>));
        }
    }
}