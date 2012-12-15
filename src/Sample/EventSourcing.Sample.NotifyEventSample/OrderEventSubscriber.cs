using System.Data;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.NotifyEventSample
{
    public class OrderEventSubscriber
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public OrderEventSubscriber(ICurrentDbTransactionProvider transactionProvider)
        {
            _transaction = transactionProvider.CurrentTransaction;
            _connection = _transaction.Connection;
        }

        [SyncEventHandler]
        public void Handle(OrderCreated evnt)
        {
            _connection.Insert(
                new { Id = evnt.Id, Customer = evnt.Customer, CreatedTime = evnt.CreatedTime },
                "eventsourcing_sample_order",
                _transaction);
        }
        [SyncEventHandler]
        public void Handle(OrderItemAdded evnt)
        {
            _connection.Insert(evnt, "eventsourcing_sample_orderitem", _transaction);
        }
        [SyncEventHandler]
        public void Handle(OrderItemAmountUpdated evnt)
        {
            _connection.Update(
                new { Amount = evnt.Amount },
                new { OrderId = evnt.OrderId, ProductId = evnt.ProductId },
                "eventsourcing_sample_orderitem",
                _transaction);
        }
        [SyncEventHandler]
        public void Handle(OrderItemRemoved evnt)
        {
            _connection.Delete(
                new { OrderId = evnt.OrderId, ProductId = evnt.ProductId },
                "eventsourcing_sample_orderitem",
                _transaction);
        }
        [SyncEventHandler]
        public void Handle(OrderItemToRemoveNotExist evnt)
        {
            var logger = ObjectContainer.Resolve<CodeSharp.EventSourcing.ILoggerFactory>().Create("OrderEventSubscriber");
            logger.Info("OrderItem to remove not exist.");
        }
    }
}
