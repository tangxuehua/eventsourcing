//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Data;

namespace CodeSharp.EventSourcing
{
    public class SqlContextTransactionManager : IContextTransactionManager, ICurrentDbTransactionProvider
    {
        private readonly string DefaultContextTransactionKey = "SqlContextTransactionKey";
        private IDbConnectionFactory _connectionFactory;
        private IContextTransactionLifetimeManager _contextTransactionLifetimeManager;
        private ILoggerFactory _loggerFactory;
        private ILogger _logger;

        public SqlContextTransactionManager(IDbConnectionFactory connectionFactory, IContextTransactionLifetimeManager contextTransactionLifetimeManager, ILoggerFactory loggerFactory)
        {
            _connectionFactory = connectionFactory;
            _contextTransactionLifetimeManager = contextTransactionLifetimeManager;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.Create("EventSourcing.SqlContextTransactionManager");
        }

        public IContextTransaction OpenContextTransaction()
        {
            var contextTransaction = _contextTransactionLifetimeManager.Find(DefaultContextTransactionKey) as IContextTransaction;

            if (contextTransaction == null)
            {
                contextTransaction = new SqlContextTransaction(_connectionFactory.OpenConnection().BeginTransaction(), _contextTransactionLifetimeManager, _loggerFactory);
                _contextTransactionLifetimeManager.Store(DefaultContextTransactionKey, contextTransaction);
                _logger.Debug("Sql context transaction created and stored.");
            }

            return contextTransaction;
        }
        public IDbTransaction CurrentTransaction
        {
            get
            {
                var sqlContextTransaction = _contextTransactionLifetimeManager.Find(DefaultContextTransactionKey) as SqlContextTransaction;
                if (sqlContextTransaction != null)
                {
                    return sqlContextTransaction.DbTransaction;
                }
                return null;
            }
        }
    }
}
