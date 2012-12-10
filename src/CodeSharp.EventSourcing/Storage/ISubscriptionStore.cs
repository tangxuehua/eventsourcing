//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 用于存储消息的订阅者地址的接口定义
    /// </summary>
    public interface ISubscriptionStore
    {
        /// <summary>
        /// Subscribes the given client address to message of the given type.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="messageType"></param>
        void Subscribe(Address address, Type messageType);
        /// <summary>
        /// Clear all the subscriptions of the given client address.
        /// </summary>
        /// <param name="address"></param>
        void ClearAddressSubscriptions(Address address);
        /// <summary>
        /// Unsubscribes the given client address from message of the given type.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="messageType"></param>
        void Unsubscribe(Address address, Type messageType);
        /// <summary>
        /// Returns a list of addresses of subscribers that previously requested to be notified
        /// of the given message type.
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        IEnumerable<Address> GetSubscriberAddressesForMessage(Type messageType);
        /// <summary>
        /// 刷新订阅者信息
        /// </summary>
        void RefreshSubscriptions();
    }
}
