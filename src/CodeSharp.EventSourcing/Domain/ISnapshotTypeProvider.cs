//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    public interface ISnapshotTypeProvider
    {
        /// <summary>
        /// 判断给定的类型是否是一个快照类.
        /// </summary>
        bool IsSnapshot(Type type);
    }
}
