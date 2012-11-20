//Copyright (c) CodeSharp.  All rights reserved.

namespace CodeSharp.EventSourcing
{
    public interface IContextManager
    {
        /// <summary>
        /// 返回一个可用的Context实例
        /// </summary>
        IContext GetContext();
    }
}
