using System;
using System.Reflection;
using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.Container.StructureMap;
using CodeSharp.EventSourcing.EventStore.NHibernate;
using CodeSharp.EventSourcing.NHibernate;
using CodeSharp.EventSourcing.SubscriptionStorage.NHibernate;

namespace EventSourcing.Sample.Test
{
    public class TestBase
    {
        private Random _random = new Random();

        public TestBase()
        {
            var modelAssembly = Assembly.Load("EventSourcing.Sample.Model");
            var applicationAssembly = Assembly.Load("EventSourcing.Sample.Application");
            var assemblies = new Assembly[] { modelAssembly, applicationAssembly };

            try
            {
                Configuration.Create("EventSourcing.Sample.Test")
                        .Initialize(new DefaultConfigurationInitializer(Assembly.GetExecutingAssembly()))
                        .Container<StructureMapObjectContainer>(new StructureMapObjectContainer())
                        .EventStore<NHibernateEventStore>()
                        .SubscriptionStorage<NHibernateSubscriptionStorage>()
                        .RegisterAggregateRootTypes(assemblies)
                        .RegisterSourcableEvents(assemblies)
                        .RegisterSourcableEventMappings(assemblies)
                        .RegisterTypeNameMappings(assemblies)
                        .RegisterAggregateRootInternalEventHandlers(assemblies)
                        .RegisterAggregateEventHandlers(assemblies)
                        .RegisterServices(assemblies)
                        .NHibernate(assemblies);
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("不可重复初始化框架配置"))
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
        }

        protected string RandomString()
        {
            return "EventSourcing_" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Ticks + _random.Next(100);
        }
    }
}
