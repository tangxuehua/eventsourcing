using System.Data;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.NotifyEventSample
{
    public class ProductEventSubscriber
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public ProductEventSubscriber(ICurrentDbTransactionProvider transactionProvider)
        {
            _transaction = transactionProvider.CurrentTransaction;
            _connection = _transaction.Connection;
        }

        [SyncEventHandler]
        public void Handle(ProductCreated evnt)
        {
            _connection.Insert(evnt, "eventsourcing_sample_product", _transaction);
        }
    }
}
