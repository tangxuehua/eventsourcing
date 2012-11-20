//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 用于标记某个方法是某个事件的同步响应函数
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SyncEventHandlerAttribute : Attribute
    {
    }
}