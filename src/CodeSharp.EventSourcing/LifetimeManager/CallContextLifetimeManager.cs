//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;

namespace CodeSharp.EventSourcing
{
    public class CallContextLifetimeManager<T> : AbstractDictStackLifetimeManager<T> where T : class, ILifetimeObject
    {
        protected override IDictionary GetDictionary()
        {
            return CallContext.GetData(DictionaryName) as IDictionary;
        }
        protected override void StoreDictionary(IDictionary dictionary)
        {
            CallContext.SetData(DictionaryName, dictionary);
        }
    }
}