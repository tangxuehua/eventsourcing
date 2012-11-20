//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections;

namespace CodeSharp.EventSourcing
{
    public abstract class AbstractStackLifetimeManager<T> : MarshalByRefObject, ILifetimeManager<T> where T : class, ILifetimeObject
    {
        protected abstract Stack GetStackFor(string key);

        public void Store(string key, ILifetimeObject obj)
        {
            var stack = GetStackFor(key);
            stack.Push(obj);
            obj.OwnerStack = stack;
        }
        public void Remove(ILifetimeObject obj)
        {
            var stack = obj.OwnerStack;

            if (stack == null || stack.Count == 0)
            {
                return;
            }

            var current = stack.Peek() as ILifetimeObject;

            if (obj != current)
            {
                throw new EventSourcingException("当前要移除的ILifetimeObject '{0}'不是其所属的Stack中的顶层元素，无法被移除");
            }

            stack.Pop();
        }
        public T Find(string key)
        {
            var stack = GetStackFor(key);

            if (stack.Count == 0)
            {
                return null;
            }

            return stack.Peek() as T;
        }
    }
}