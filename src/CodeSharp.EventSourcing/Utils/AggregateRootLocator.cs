//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 内部辅助类，用于获取某个聚合根
    /// </summary>
    internal class AggregateRootLocator
    {
        private IContextManager _contextManager = ObjectContainer.Resolve<IContextManager>();
        private static readonly AggregateRootLocator _instance = new AggregateRootLocator();
        private static readonly MethodInfo _internalMethodToGetAggregateRoot = typeof(AggregateRootLocator).GetMethod("InternalGetAggregateRoot", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        public static AggregateRoot GetAggregateRoot(Type aggregateRootType, object id)
        {
            return _internalMethodToGetAggregateRoot.MakeGenericMethod(aggregateRootType).Invoke(_instance, new object[] { id }) as AggregateRoot;
        }

        private TAggregateRoot InternalGetAggregateRoot<TAggregateRoot>(object id) where TAggregateRoot : AggregateRoot
        {
            using (var context = _contextManager.GetContext())
            {
                return context.Load<TAggregateRoot>(id);
            }
        }
    }
}
