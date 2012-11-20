using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate;
using NHibernate.Criterion;

namespace CodeSharp.EventSourcing.NHibernate
{
    /// <summary>
    /// 一个接口，定义了各种基于Session实现的常用数据库操作，一般在EventSubscriber中使用这个接口；
    /// 如果只是为了查询显示数据，建议使用IDapperQueryService
    /// </summary>
    public interface ISessionHelper
    {
        /// <summary>
        /// 在事务内执行一个给定的操作
        /// </summary>
        /// <param name="action"></param>
        void ExecuteAction(Action<ISession> action);
        /// <summary>
        /// 查询单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        T QueryUnique<T>(ISession session, object param) where T : class;
        /// <summary>
        /// 查询多个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<T> QueryList<T>(ISession session, object param) where T : class;
        /// <summary>
        /// 删除符合条件的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="param"></param>
        void Delete<T>(ISession session, object param) where T : class;
        /// <summary>
        /// 查询单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="criterion"></param>
        /// <returns></returns>
        T QueryUnique<T>(ISession session, ICriterion criterion) where T : class;
        /// <summary>
        /// 查询多个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="criterion"></param>
        /// <returns></returns>
        IEnumerable<T> QueryList<T>(ISession session, ICriterion criterion) where T : class;
        /// <summary>
        /// 删除符合条件的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="criterion"></param>
        void Delete<T>(ISession session, ICriterion criterion) where T : class;
        /// <summary>
        /// 根据参数创建一个查询对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        ICriterion CreateCriterion(object param);
    }

    public class SessionHelper : ISessionHelper
    {
        private ISessionFactory _sessionFactory;
        private ILogger _logger;

        public SessionHelper(ISessionFactory sessionFactory, ILoggerFactory loggerFactory)
        {
            _sessionFactory = sessionFactory;
            _logger = loggerFactory.Create("CodeSharp.EventSourcing.NHibernate");
        }

        public void ExecuteAction(Action<ISession> action)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        action(session);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.Error(ex);
                        throw;
                    }
                }
            }
        }
        public T QueryUnique<T>(ISession session, object param) where T : class
        {
            return QueryUnique<T>(session, CreateCriterion(param));
        }
        public IEnumerable<T> QueryList<T>(ISession session, object param) where T : class
        {
            return QueryList<T>(session, CreateCriterion(param));
        }
        public void Delete<T>(ISession session, object param) where T : class
        {
            Delete<T>(session, CreateCriterion(param));
        }
        public T QueryUnique<T>(ISession session, ICriterion criterion) where T : class
        {
            return session.CreateCriteria<T>().Add(criterion).UniqueResult<T>();
        }
        public IEnumerable<T> QueryList<T>(ISession session, ICriterion criterion) where T : class
        {
            return session.CreateCriteria<T>().Add(criterion).List<T>();
        }
        public void Delete<T>(ISession session, ICriterion criterion) where T : class
        {
            var result = session.CreateCriteria<T>().Add(criterion).List<T>();
            foreach (var obj in result)
            {
                session.Delete(obj);
            }
        }
        public ICriterion CreateCriterion(object param)
        {
            ICriterion criterion = null;
            var properties = param.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            for (var index = 0; index < properties.Count(); index++)
            {
                var property = properties.ElementAt(index);
                if (index == 0)
                {
                    criterion = Expression.Eq(property.Name, property.GetValue(param, null));
                }
                else
                {
                    criterion = Expression.And(criterion, Expression.Eq(property.Name, property.GetValue(param, null)));
                }
            }

            return criterion;
        }
    }
}
