using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.NHibernate;
using EventSourcing.Sample.Entities;
using EventSourcing.Sample.Model.Orders;

namespace EventSourcing.Sample.EventSubscribers
{
    public class OrderEventSubscriber
    {
        private ISessionHelper _sessionHelper;

        public OrderEventSubscriber(ISessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
        }

        [AsyncEventHandler]
        public void Handle(OrderCreated evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                session.Save(ObjectHelper.CreateObject<OrderEntity>(evnt));
            });
        }
        [AsyncEventHandler]
        public void Handle(OrderItemAdded evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                session.Save(ObjectHelper.CreateObject<OrderItemEntity>(evnt));
            });
        }
        [AsyncEventHandler]
        public void Handle(OrderItemAmountUpdated evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                var orderItem = _sessionHelper.QueryUnique<OrderItemEntity>(session, new { OrderId = evnt.OrderId, ProductId = evnt.ProductId });
                ObjectHelper.UpdateObject<OrderItemEntity, OrderItemAmountUpdated>(orderItem, evnt, x => x.Amount);
                session.Update(orderItem);
            });
        }
        [AsyncEventHandler]
        public void Handle(OrderItemRemoved evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                _sessionHelper.Delete<OrderItemEntity>(session, new { OrderId = evnt.OrderId, ProductId = evnt.ProductId });
            });
        }
        [AsyncEventHandler]
        public void Handle(OrderItemToRemoveNotExist evnt)
        {
            var logger = ObjectContainer.Resolve<CodeSharp.EventSourcing.ILoggerFactory>().Create("OrderEventSubscriber");
            logger.Error("OrderItem to remove not exist.");
        }
    }
}
