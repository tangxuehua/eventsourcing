//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 日志记录器工厂接口定义
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// 创建Logger
        /// </summary>
        ILogger Create(string name);
        /// <summary>
        /// 创建Logger
        /// </summary>
        ILogger Create(Type type);
    }
}
