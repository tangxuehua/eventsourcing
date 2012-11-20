//Copyright (c) CodeSharp.  All rights reserved.

using System.Collections;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 表示一个被ILifetimeManager管理的对象
    /// </summary>
    public interface ILifetimeObject
    {
        /// <summary>
        /// 表示实际存放当前对象的一个Stack对象
        /// </summary>
        Stack OwnerStack { get; set; }
    }
}
