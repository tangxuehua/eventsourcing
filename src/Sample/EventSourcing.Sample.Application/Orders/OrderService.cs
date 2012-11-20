using System;
using CodeSharp.EventSourcing;
using EventSourcing.Sample.Model.Orders;

namespace EventSourcing.Sample.Application
{
    public interface IOrderService
    {
        Order Create(string customer);
        OrderItem AddItem(Guid orderId, Guid productId, int amount);
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

        public Order Create(string customer)
        {
            using (var context = _contextManager.GetContext())
            {
                var order = new Order(customer);
                context.Add(order);
                context.SaveChanges();
                return order;
            }
        }
        public OrderItem AddItem(Guid orderId, Guid productId, int amount)
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
