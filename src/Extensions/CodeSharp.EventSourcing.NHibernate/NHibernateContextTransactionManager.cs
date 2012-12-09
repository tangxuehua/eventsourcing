//Copyright (c) CodeSharp.  All rights reserved.

using NHibernate;

namespace CodeSharp.EventSourcing
{
    public interface ICurrentSessionProvider
    {
        ISession CurrentSession { get; }
    }
    public class NHibernateContextTransactionManager : IContextTransactionManager, ICurrentSessionProvider
    {
        private readonly string DefaultContextTransactionKey = "NHibernateContextTransactionKey";
        private ISessionFactory _sessionFactory;
        private IContextTransactionLifetimeManager _contextTransactionLifetimeManager;
        private ILoggerFactory _loggerFactory;
        private ILogger _logger;

        public ISession CurrentSession
        {
            get
            {
                var nhibernateContextTransaction = _contextTransactionLifetimeManager.Find(DefaultContextTransactionKey) as NHibernateContextTransaction;
                if (nhibernateContextTransaction != null)
                {
                    return nhibernateContextTransaction.Session;
                }
                return null;
            }
        }

        public NHibernateContextTransactionManager(ISessionFactory sessionFactory, IContextTransactionLifetimeManager contextTransactionLifetimeManager, ILoggerFactory loggerFactory)
        {
            _sessionFactory = sessionFactory;
            _contextTransactionLifetimeManager = contextTransactionLifetimeManager;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.Create("EventSourcing.NHibernateContextTransactionManager");
        }

        public IContextTransaction OpenContextTransaction()
        {
            var contextTransaction = _contextTransactionLifetimeManager.Find(DefaultContextTransactionKey) as IContextTransaction;

            if (contextTransaction == null)
            {
                var session = _sessionFactory.OpenSession();
                session.BeginTransaction();
                contextTransaction = new NHibernateContextTransaction(session, _contextTransactionLifetimeManager, _loggerFactory);
                _contextTransactionLifetimeManager.Store(DefaultContextTransactionKey, contextTransaction);
                _logger.Debug("NHibernate context transaction created and stored.");
            }

            return contextTransaction;
        }
    }
}
