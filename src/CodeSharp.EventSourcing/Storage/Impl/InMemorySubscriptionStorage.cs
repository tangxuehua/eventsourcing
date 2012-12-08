//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// In memory implementation of the subscription storage.
    /// </summary>
    public class InMemorySubscriptionStorage : ISubscriptionStorage
    {
        private readonly ConcurrentDictionary<Type, List<Address>> _storage = new ConcurrentDictionary<Type, List<Address>>();

        public void Subscribe(Address address, Type messageType)
        {
            if (!_storage.ContainsKey(messageType))
            {
                _storage[messageType] = new List<Address>();
            }
            if (!_storage[messageType].Contains(address))
            {
                _storage[messageType].Add(address);
            }
        }
        public void ClearAddressSubscriptions(Address address)
        {
            foreach (var messageType in _storage.Keys)
            {
                var addressArray = new Address[_storage[messageType].Count];
                _storage[messageType].CopyTo(addressArray);
                foreach (var subscriberAddress in addressArray)
                {
                    if (subscriberAddress == address)
                    {
                        _storage[messageType].Remove(subscriberAddress);
                    }
                }
            }
        }
        public void Unsubscribe(Address address, Type messageType)
        {
            if (_storage.ContainsKey(messageType) && _storage[messageType].Contains(address))
            {
                _storage[messageType].Remove(address);
            }
        }
        public IEnumerable<Address> GetSubscriberAddressesForMessage(Type messageType)
        {
            if (_storage.ContainsKey(messageType))
            {
                return _storage[messageType];
            }
            return new List<Address>();
        }
        public void RefreshSubscriptions()
        {
        }
    }
}
