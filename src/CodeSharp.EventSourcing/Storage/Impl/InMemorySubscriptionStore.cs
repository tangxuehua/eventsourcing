//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// In memory implementation of the subscription store.
    /// </summary>
    public class InMemorySubscriptionStore : ISubscriptionStore
    {
        private readonly ConcurrentDictionary<Type, List<Address>> _subscriptionDictionary = new ConcurrentDictionary<Type, List<Address>>();

        public void Subscribe(Address address, Type messageType)
        {
            if (!_subscriptionDictionary.ContainsKey(messageType))
            {
                _subscriptionDictionary[messageType] = new List<Address>();
            }
            if (!_subscriptionDictionary[messageType].Contains(address))
            {
                _subscriptionDictionary[messageType].Add(address);
            }
        }
        public void ClearAddressSubscriptions(Address address)
        {
            foreach (var messageType in _subscriptionDictionary.Keys)
            {
                var addressArray = new Address[_subscriptionDictionary[messageType].Count];
                _subscriptionDictionary[messageType].CopyTo(addressArray);
                foreach (var subscriberAddress in addressArray)
                {
                    if (subscriberAddress == address)
                    {
                        _subscriptionDictionary[messageType].Remove(subscriberAddress);
                    }
                }
            }
        }
        public void Unsubscribe(Address address, Type messageType)
        {
            if (_subscriptionDictionary.ContainsKey(messageType) && _subscriptionDictionary[messageType].Contains(address))
            {
                _subscriptionDictionary[messageType].Remove(address);
            }
        }
        public IEnumerable<Address> GetSubscriberAddressesForMessage(Type messageType)
        {
            if (_subscriptionDictionary.ContainsKey(messageType))
            {
                return _subscriptionDictionary[messageType];
            }
            return new List<Address>();
        }
        public void RefreshSubscriptions()
        {
        }
    }
}
