//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 定义一个接口，如果某个聚合根类实现该接口，表明该聚合根支持创建快照以及从快照还原
    /// </summary>
    public interface ISnapshotable<TSnapshot>
    {
        TSnapshot CreateSnapshot();
        void RestoreFromSnapshot(TSnapshot snapshot);
    }
}
