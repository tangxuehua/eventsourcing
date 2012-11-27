//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    public interface IContextTransactionManager
    {
        /// <summary>
        /// 返回一个可用的Context事务
        /// </summary>
        /// <returns></returns>
        IContextTransaction OpenContextTransaction();
    }
}
