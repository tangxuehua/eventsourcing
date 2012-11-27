using System;
using System.Reflection;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.Test
{
    public class TestBase
    {
        private Random _random = new Random();

        public TestBase()
        {
            var modelAssembly = Assembly.Load("EventSourcing.Sample.Model");
            var applicationAssembly = Assembly.Load("EventSourcing.Sample.Application");
            var eventSubscriberAssembly = Assembly.Load("EventSourcing.Sample.EventSubscribers");
            var assemblies = new Assembly[] { modelAssembly, applicationAssembly, eventSubscriberAssembly };
            var configAssembly = Assembly.GetExecutingAssembly();
            try
            {
                Configuration.Config("EventSourcing.Sample.Test", configAssembly, assemblies);
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
