//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace CodeSharp.EventSourcing
{
    public abstract class AbstractDictStackLifetimeManager<T> : AbstractStackLifetimeManager<T> where T : class, ILifetimeObject
    {
        protected readonly string DictionaryName = Guid.NewGuid().ToString();

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override Stack GetStackFor(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            var stackDictionary = GetDictionary();

            if (stackDictionary == null)
            {
                stackDictionary = new HybridDictionary(true);
                StoreDictionary(stackDictionary);
            }

            var stack = stackDictionary[key] as Stack;

            if (stack == null)
            {
                stack = Stack.Synchronized(new Stack());
                stackDictionary[key] = stack;
            }

            return stack;
        }

        protected abstract IDictionary GetDictionary();
        protected abstract void StoreDictionary(IDictionary dictionary);
    }
}