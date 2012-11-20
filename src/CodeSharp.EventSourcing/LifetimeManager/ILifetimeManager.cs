//Copyright (c) CodeSharp.  All rights reserved.

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 对象生命周期管理器接口定义
    /// </summary>
    public interface ILifetimeManager<out T> where T : class, ILifetimeObject
    {
        /// <summary>
        /// 存储某个对象
        /// </summary>
        void Store(string key, ILifetimeObject obj);
        /// <summary>
        /// 移除某个对象
        /// </summary>
        void Remove(ILifetimeObject obj);
        /// <summary>
        /// 根据key返回一个可用的对象
        /// </summary>
        T Find(string key);
    }
}
