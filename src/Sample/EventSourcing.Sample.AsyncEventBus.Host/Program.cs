using System;
using System.Reflection;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.AsyncEventBus.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Configuration
                .Config("EventSourcing.Sample.AsyncEventBus.Host", assembly, assembly)
                .StartAsyncEventSubscriberEndpoint();
            Console.WriteLine("EventSourcing.Sample.AsyncEventBus.Host started, press Enter to exit.");
            Console.ReadLine();
        }
    }
}
