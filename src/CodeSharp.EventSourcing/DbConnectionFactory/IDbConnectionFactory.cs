//Copyright (c) CodeSharp.  All rights reserved.

using System.Data;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 数据库连接工厂接口定义
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// 打开一个新的数据库连接
        /// </summary>
        /// <returns></returns>
        IDbConnection OpenConnection();
    }
}