//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 日志记录器接口定义
    /// </summary>
    public interface ILogger
    {
        #region Info
        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="message">错误内容</param>
        void Info(object message);
        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="format">内容格式</param>
        /// <param name="args">参数</param>
        void InfoFormat(string format, params object[] args);
        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="message">错误内容</param>
        /// <param name="exception">异常</param>
        void Info(object message, Exception exception);
        #endregion

        #region Error
        /// <summary>
        /// 记录错误
        /// </summary>
        /// <param name="message">内容格式</param>
        void Error(object message);
        /// <summary>
        /// 记录错误
        /// </summary>
        /// <param name="format">内容格式</param>
        /// <param name="args">参数</param>
        void ErrorFormat(string format, params object[] args);
        /// <summary> 
        /// 记录错误
        /// </summary>
        /// <param name="message">错误内容</param>
        /// <param name="exception">异常</param>
        void Error(object message, Exception exception);
        #endregion

        #region Warn
        /// <summary>
        /// 记录警告
        /// </summary>
        /// <param name="message">内容格式</param>
        void Warn(object message);
        /// <summary>
        /// 记录警告
        /// </summary>
        /// <param name="format">内容格式</param>
        /// <param name="args">参数</param>
        void WarnFormat(string format, params object[] args);
        /// <summary>
        /// 记录警告
        /// </summary>
        /// <param name="message">错误内容</param>
        /// <param name="exception">异常</param>
        void Warn(object message, Exception exception);
        #endregion

        #region Debug

        bool IsDebugEnabled { get; }
        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="message">内容格式</param>
        void Debug(object message);
        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="format">内容格式</param>
        /// <param name="args">参数</param>
        void DebugFormat(string format, params object[] args);
        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="message">错误内容</param>
        /// <param name="exception">异常</param>
        void Debug(object message, Exception exception);
        #endregion

        #region Fatal
        /// <summary>
        /// 记录致命错误
        /// </summary>
        /// <param name="message">内容格式</param>
        void Fatal(object message);
        /// <summary>
        /// 记录致命错误
        /// </summary>
        /// <param name="format">内容格式</param>
        /// <param name="args">参数</param>
        void FatalFormat(string format, params object[] args);
        /// <summary> 
        /// 记录致命错误
        /// </summary>
        /// <param name="message">错误内容</param>
        /// <param name="exception">异常</param>
        void Fatal(object message, Exception exception);
        #endregion
    }
}