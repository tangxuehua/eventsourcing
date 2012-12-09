//Copyright (c) CodeSharp.  All rights reserved.

using System.Collections;
using System.Data;
using NHibernate;

namespace CodeSharp.EventSourcing
{
    public class NHibernateContextTransaction : IContextTransaction
    {
        private ISession _session;
        private IContextTransactionLifetimeManager _transactionLifetimeManager;
        private ILogger _logger;

        public Stack OwnerStack { get; set; }
        public ISession Session { get { return _session; } }

        public NHibernateContextTransaction(ISession session, IContextTransactionLifetimeManager transactionLifetimeManager, ILoggerFactory loggerFactory)
        {
            _session = session;
            _transactionLifetimeManager = transactionLifetimeManager;
            _logger = loggerFactory.Create("EventSourcing.NHibernateContextTransaction");
        }

        public void Commit()
        {
            try
            {
                _session.Flush();
                _session.Transaction.Commit();
            }
            catch (StaleObjectStateException ex)
            {
                throw new ConcurrencyException(ConcurrencyException.DefaultConcurrencyExceptionMessage, ex);
            }
        }
        public void Rollback()
        {
            _session.Transaction.Rollback();
        }
        public void Dispose()
        {
            _session.Dispose();
            _transactionLifetimeManager.Remove(this);
            _logger.Debug("NHibernate context transaction removed and disposed.");
        }
    }
}
