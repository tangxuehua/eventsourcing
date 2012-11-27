using System;
using System.Reflection;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.Host
{
    /// <summary>
    /// 实现一个简单的事件订阅者端点的宿主
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            ConfigEventSourcing();
            ObjectContainer.Resolve<ILoggerFactory>().Create("EventSourcing.Sample.Host").Info("EventSourcing.Sample.Host started, press enter to exit.");
            Console.ReadLine();
        }

        private static void ConfigEventSourcing()
        {
            var eventSubscriberAssembly = Assembly.Load("EventSourcing.Sample.EventSubscribers");
            var configAssembly = Assembly.GetExecutingAssembly();

            Configuration.Config("EventSourcing.Sample.Host", configAssembly, eventSubscriberAssembly).StartEventSubscriberEndpoint();
        }
    }
}
