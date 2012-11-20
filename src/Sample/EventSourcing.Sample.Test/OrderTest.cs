using System.Linq;
using CodeSharp.EventSourcing;
using EventSourcing.Sample.Application;
using EventSourcing.Sample.Model.Orders;
using NUnit.Framework;

namespace EventSourcing.Sample.Test
{
    [TestFixture]
    public class OrderTest : TestBase
    {
        [Test]
        public void Test()
        {
            var productService = ObjectContainer.Resolve<IProductService>();
            var orderService = ObjectContainer.Resolve<IOrderService>();
            var sessionManager = ObjectContainer.Resolve<IContextManager>();

            //创建商品
            var product1 = productService.Create(RandomString(), RandomString(), 10);
            var product2 = productService.Create(RandomString(), RandomString(), 20);
            var product3 = productService.Create(RandomString(), RandomString(), 30);

            //创建并更新订单
            var order = orderService.Create("tangxuehua");
            var item1 = orderService.AddItem(order.Id, product1.Id, 1);
            var item2 = orderService.AddItem(order.Id, product2.Id, 2);
            var item3 = orderService.AddItem(order.Id, product3.Id, 3);
            orderService.UpdateOrderItemAmount(order.Id, product2.Id, 4);
            orderService.RemoveOrderItem(order.Id, product3.Id);
            orderService.RemoveOrderItem(order.Id, product3.Id);

            //重新获取订单
            using (var session = sessionManager.GetContext())
            {
                order = session.Load<Order>(order.Id);
            }
            //Assert结果
            Assert.AreEqual(2, order.Items.Count());
            Assert.AreEqual(90, order.TotalPrice);
            var item = order.GetItem(product1.Id);
            Assert.IsNotNull(item);
            item = order.GetItem(product2.Id);
            Assert.IsNotNull(item);
            Assert.AreEqual(4, item.Amount);
            item = order.GetItem(product3.Id);
            Assert.IsNull(item);
        }
    }
}
