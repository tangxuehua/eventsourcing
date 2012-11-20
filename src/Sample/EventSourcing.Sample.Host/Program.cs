using System;
using System.Reflection;
using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.Container.StructureMap;
using CodeSharp.EventSourcing.NHibernate;
using CodeSharp.EventSourcing.SubscriptionStorage.NHibernate;

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
            var entityAssembly = Assembly.Load("EventSourcing.Sample.Entities");
            var mappingAssembly = Assembly.Load("EventSourcing.Sample.Entities.Mappings");
            var eventSubscriberAssembly = Assembly.Load("EventSourcing.Sample.EventSubscribers");
            var assemblies = new Assembly[] { entityAssembly, mappingAssembly, eventSubscriberAssembly };

            Configuration
                .Create("EventSourcing.Sample.Host")
                .Initialize(new DefaultConfigurationInitializer(Assembly.GetExecutingAssembly()))
                .Container<StructureMapObjectContainer>(new StructureMapObjectContainer())
                .SubscriptionStorage<NHibernateSubscriptionStorage>()
                .RegisterAsyncEventHandlers(assemblies)
                .RegisterEventSubscribers(assemblies)
                .NHibernate(assemblies)
                .StartEventSubscriberEndpoint();
        }
    }
}
