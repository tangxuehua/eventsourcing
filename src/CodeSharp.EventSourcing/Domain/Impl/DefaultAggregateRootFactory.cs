//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 默认的聚合根工厂
    /// </summary>
    public class DefaultAggregateRootFactory : IAggregateRootFactory
    {
        public AggregateRoot CreateAggregateRoot(Type aggregateRootType)
        {
            if (!typeof(AggregateRoot).IsAssignableFrom(aggregateRootType))
            {
                var message = string.Format("您要创建的聚合根（{0}）没有继承AggregateRoot基类", aggregateRootType.FullName);
                throw new EventSourcingException(message);
            }

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var constructor = aggregateRootType.GetConstructor(flags, null, Type.EmptyTypes, null);
            if (constructor == null)
            {
                var message = string.Format("没有在类型'{0}'上找到默认无参的构造函数", aggregateRootType.FullName);
                throw new EventSourcingException(message);
            }

            return constructor.Invoke(null) as AggregateRoot;
        }
    }
}
