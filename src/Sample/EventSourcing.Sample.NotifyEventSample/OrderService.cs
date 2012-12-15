using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.NotifyEventSample
{
    public interface IOrderService
    {
        Order CreateOrder(string customer);
        OrderItem AddOrderItem(Guid orderId, Guid productId, int amount);
        void UpdateOrderItemAmount(Guid orderId, Guid productId, int amount);
        void RemoveOrderItem(Guid orderId, Guid productId);
    }
    public class OrderService : IOrderService
    {
        private IContextManager _contextManager;

        public OrderService(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public Order CreateOrder(string customer)
        {
            using (var context = _contextManager.GetContext())
            {
                var order = new Order(customer);
                context.Add(order);
                context.SaveChanges();
                return order;
            }
        }
        public OrderItem AddOrderItem(Guid orderId, Guid productId, int amount)
        {
            using (var context = _contextManager.GetContext())
            {
                var product = context.Load<Product>(productId);
                var order = context.Load<Order>(orderId);
                var orderItem = order.AddItem(product, amount);
                context.SaveChanges();
                return orderItem;
            }
        }
        public void RemoveOrderItem(Guid orderId, Guid productId)
        {
            using (var context = _contextManager.GetContext())
            {
                var order = context.Load<Order>(orderId);
                order.RemoveItem(productId);
                context.SaveChanges();
            }
        }
        public void UpdateOrderItemAmount(Guid orderId, Guid productId, int amount)
        {
            using (var context = _contextManager.GetContext())
            {
                var order = context.Load<Order>(orderId);
                order.UpdateItemAmount(productId, amount);
                context.SaveChanges();
            }
        }
    }
}
