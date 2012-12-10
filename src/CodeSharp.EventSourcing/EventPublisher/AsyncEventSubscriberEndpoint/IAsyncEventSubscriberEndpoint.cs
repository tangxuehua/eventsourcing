//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 定义异步EventPublisher事件订阅者端点的接口，一个事件订阅者端点可以接收来自某个特定队列地址的消息
    /// </summary>
    public interface IAsyncEventSubscriberEndpoint
    {
        /// <summary>
        /// 初始化当前事件订阅者端点
        /// </summary>
        /// <param name="address"></param>
        /// <param name="clearSubscriptions"></param>
        void Initialize(string address, bool clearSubscriptions);
        /// <summary>
        /// 启动当前事件订阅者端点，开始接收当前端点的地址对应队列的消息
        /// </summary>
        void Start();
        /// <summary>
        /// 停止当前事件订阅者端点，停止接收当前端点的地址对应队列的消息
        /// </summary>
        void Stop();
    }
}
