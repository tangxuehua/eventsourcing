using System;
using System.Reflection;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.NotifyEventSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Configuration.Config("EventSourcing.Sample.NotifyEventSample", assembly, assembly);

            var productService = ObjectContainer.Resolve<IProductService>();
            var orderService = ObjectContainer.Resolve<IOrderService>();

            //创建商品
            var product1 = productService.Create("Sample Product1", "Description of Sample Product1", 10);
            var product2 = productService.Create("Sample Product2", "Description of Sample Product2", 20);
            var product3 = productService.Create("Sample Product3", "Description of Sample Product3", 30);

            //创建并更新订单
            var order = orderService.CreateOrder("tangxuehua");
            var item1 = orderService.AddOrderItem(order.Id, product1.Id, 1);
            var item2 = orderService.AddOrderItem(order.Id, product2.Id, 2);
            var item3 = orderService.AddOrderItem(order.Id, product3.Id, 3);
            orderService.UpdateOrderItemAmount(order.Id, product2.Id, 4);
            orderService.RemoveOrderItem(order.Id, product3.Id);
            orderService.RemoveOrderItem(order.Id, product3.Id);

            Console.Write("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
