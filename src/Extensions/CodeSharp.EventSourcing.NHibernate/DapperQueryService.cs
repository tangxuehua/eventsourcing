//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using NHibernate;

namespace CodeSharp.EventSourcing.NHibernate
{
    /// <summary>
    /// 一个基于dapper组件实现的高性能查询服务接口，建议在查询要显示的数据时使用该接口
    /// </summary>
    public interface IDapperQueryService
    {
        /// <summary>
        /// 根据给定的SQL以及参数查询指定类型的结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(string sql, object param);
        /// <summary>
        /// 根据给定的查询委托进行查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        T Query<T>(Func<IDbConnection, T> func);
    }
    public class DapperQueryService : IDapperQueryService
    {
        private ISessionFactory _sessionFactory;

        public DapperQueryService(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public IEnumerable<T> Query<T>(string sql, object param)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                return session.Connection.Query<T>(sql, param);
            }
        }
        public T Query<T>(Func<IDbConnection, T> func)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                return func(session.Connection);
            }
        }
    }
}
