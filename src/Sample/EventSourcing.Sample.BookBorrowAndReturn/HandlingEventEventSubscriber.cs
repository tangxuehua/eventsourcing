using System.Data;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.BookBorrowAndReturn
{
    public class HandlingEventEventSubscriber
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public HandlingEventEventSubscriber(ICurrentDbTransactionProvider transactionProvider)
        {
            _transaction = transactionProvider.CurrentTransaction;
            _connection = _transaction.Connection;
        }

        [SyncEventHandler]
        private void Handle(HandlingEventCreated evnt)
        {
            _connection.Insert(evnt, "EventSourcing_Sample_HandlingEvent", _transaction);
        }
    }
}
