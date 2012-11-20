//Copyright (c) CodeSharp.  All rights reserved.

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 根据当前应用环境动态管理Context生命周期的生命周期管理器
    /// </summary>
    public class DynamicContextLifetimeManager : DynamicLifetimeManager<IContext>, IContextLifetimeManager
    {
    }
}
