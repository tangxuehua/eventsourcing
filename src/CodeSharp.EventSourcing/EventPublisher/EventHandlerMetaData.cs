//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 事件订阅者元数据信息
    /// </summary>
    public class EventHandlerMetaData
    {
        /// <summary>
        /// 订阅者类型
        /// </summary>
        public Type SubscriberType { get; set; }
        /// <summary>
        /// 事件响应方法
        /// </summary>
        public MethodInfo EventHandler { get; set; }
    }
}
