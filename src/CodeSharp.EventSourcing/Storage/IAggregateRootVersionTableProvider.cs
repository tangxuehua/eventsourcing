//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 定义用于提供聚合根的事件版本信息与对应存放的数据库表的接口
    /// </summary>
    public interface IAggregateRootVersionTableProvider
    {
        void AddMapping(Type aggregateRootType, string table);
        string GetTable(Type aggregateRootType);
    }
}
