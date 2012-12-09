//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Data;
using System.Collections;

namespace CodeSharp.EventSourcing
{
    public class SqlContextTransaction : IContextTransaction
    {
        private IDbTransaction _transaction;
        private IContextTransactionLifetimeManager _transactionLifetimeManager;
        private ILogger _logger;

        public IDbTransaction DbTransaction { get { return _transaction; } }
        public Stack OwnerStack { get; set; }

        public SqlContextTransaction(IDbTransaction transaction, IContextTransactionLifetimeManager transactionLifetimeManager, ILoggerFactory loggerFactory)
        {
            _transaction = transaction;
            _transactionLifetimeManager = transactionLifetimeManager;
            _logger = loggerFactory.Create("EventSourcing.SqlContextTransaction");
        }

        public void Commit()
        {
            _transaction.Commit();
        }
        public void Rollback()
        {
            _transaction.Rollback();
        }
        public void Dispose()
        {
            _transaction.Dispose();
            _transactionLifetimeManager.Remove(this);
            _logger.Debug("Sql context transaction removed and disposed.");
        }
    }
}
