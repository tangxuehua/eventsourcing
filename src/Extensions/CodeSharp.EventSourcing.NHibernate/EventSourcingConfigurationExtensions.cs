//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using FluentNHibernate;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernateCfg = NHibernate.Cfg;

namespace CodeSharp.EventSourcing.NHibernate
{
    /// <summary>
    /// Extension methods to extend event sourcing Configruation class.
    /// </summary>
    public static class EventSourcingConfigurationExtensions
    {
        /// <summary>
        /// 配置NHibernate框架
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static Configuration NHibernate(this Configuration configuration, params Assembly[] assemblies)
        {
            configuration
                .BuildNHibernateConfiguration()
                .AddUserDefinedNHibernateMappings(assemblies)
                .AddSourcableEventNHibernateMappings(assemblies)
                .AddAggregateRootVersionNHibernateMappings(assemblies)
                .AddSubscriptionNHibernateMappings()
                .BuildNHibernateSessionFactory();

            configuration.ContextTransactionManager<NHibernateContextTransactionManager>();
            configuration.CurrentSessionProvider<NHibernateContextTransactionManager>();
            configuration.EventStore<NHibernateEventStore>();
            configuration.SnapshotStore<NHibernateSnapshotStore>();
            configuration.SubscriptionStore<NHibernateSubscriptionStore>();

            return configuration;
        }
        /// <summary>
        /// 注册当前Session提供者实现类
        /// </summary>
        public static Configuration CurrentSessionProvider<T>(this Configuration configuration) where T : class, ICurrentSessionProvider
        {
            ObjectContainer.Register<ICurrentSessionProvider, T>(LifeStyle.Transient);
            return configuration;
        }
        /// <summary>
        /// 构建一个NHibernate Configuration实例并注册到容器
        /// </summary>
        /// <returns></returns>
        public static Configuration BuildNHibernateConfiguration(this Configuration configuration)
        {
            var nhibernateConfiguration = new NHibernateCfg.Configuration();
            nhibernateConfiguration.SetProperties(GetConfigSettings());
            ObjectContainer.Register(nhibernateConfiguration);
            return configuration;
        }
        /// <summary>
        /// 将给定程序集中的所有的用户写的FluentNHibernate Mappings添加到NHibernate Configuration中
        /// </summary>
        public static Configuration AddUserDefinedNHibernateMappings(this Configuration configuration, params Assembly[] assemblies)
        {
            var nhibernateConfiguration = ObjectContainer.Resolve<NHibernateCfg.Configuration>();
            foreach (var assembly in assemblies)
            {
                nhibernateConfiguration.AddMappingsFromAssembly(assembly);
            }
            return configuration;
        }
        /// <summary>
        /// 为给定程序集中的所有的聚合根产生的可溯源事件与要保存的表建立NHibernate映射
        /// </summary>
        public static Configuration AddSourcableEventNHibernateMappings(this Configuration configuration, params Assembly[] assemblies)
        {
            var nhibernateConfiguration = ObjectContainer.Resolve<NHibernateCfg.Configuration>();
            var eventTable = nhibernateConfiguration.Properties["eventTable"];
            if (!string.IsNullOrEmpty(eventTable))
            {
                foreach (var assembly in assemblies)
                {
                    var mapper = new ModelMapper();
                    foreach (var type in assembly.GetTypes().Where(x => TypeUtils.IsAggregateRoot(x)))
                    {
                        var mappingType = typeof(SourcableEventMapping<>).MakeGenericType(type);
                        var mapping = Activator.CreateInstance(mappingType, eventTable) as IConformistHoldersProvider;
                        mapper.AddMapping(mapping);
                    }
                    nhibernateConfiguration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
                }
            }
            return configuration;
        }
        /// <summary>
        /// 为给定程序集中的所有的聚合根的版本信息与要保存的表建立NHibernate映射
        /// </summary>
        public static Configuration AddAggregateRootVersionNHibernateMappings(this Configuration configuration, params Assembly[] assemblies)
        {
            var nhibernateConfiguration = ObjectContainer.Resolve<NHibernateCfg.Configuration>();
            var versionTable = nhibernateConfiguration.Properties["versionTable"];
            if (!string.IsNullOrEmpty(versionTable))
            {
                foreach (var assembly in assemblies)
                {
                    var mapper = new ModelMapper();
                    foreach (var type in assembly.GetTypes().Where(x => TypeUtils.IsAggregateRoot(x)))
                    {
                        var mappingType = typeof(AggregateRootVersionMapping<>).MakeGenericType(type);
                        var mapping = Activator.CreateInstance(mappingType, versionTable) as IConformistHoldersProvider;
                        mapper.AddMapping(mapping);
                    }
                    nhibernateConfiguration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
                }
            }
            return configuration;
        }
        /// <summary>
        /// 为事件订阅者信息与要保存的表建立NHibernate映射
        /// </summary>
        public static Configuration AddSubscriptionNHibernateMappings(this Configuration configuration)
        {
            var nhibernateConfiguration = ObjectContainer.Resolve<NHibernateCfg.Configuration>();
            var subscriptionTable = nhibernateConfiguration.Properties["subscriptionTable"];
            if (!string.IsNullOrEmpty(subscriptionTable))
            {
                var mapper = new ModelMapper();
                var mapping = Activator.CreateInstance(typeof(SubscriptionMapping), subscriptionTable) as IConformistHoldersProvider;
                mapper.AddMapping(mapping);
                nhibernateConfiguration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
            }
            return configuration;
        }
        /// <summary>
        /// 构建一个NHibernate SessionFactory实例并注册到容器
        /// </summary>
        /// <returns></returns>
        public static Configuration BuildNHibernateSessionFactory(this Configuration configuration)
        {
            var sessionFactory = ObjectContainer.Resolve<NHibernateCfg.Configuration>().BuildSessionFactory();
            ObjectContainer.Register<ISessionFactory>(sessionFactory);
            return configuration;
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <returns></returns>
        private static IDictionary<string, string> GetConfigSettings()
        {
            var settings = new Dictionary<string, string>();
            var configFile = Configuration.Instance.GetSetting<string>("nhibernateConfigFile");
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);
            XmlElement element = null;

            if (new FileInfo(path).Exists)
            {
                var document = new XmlDocument();
                document.Load(path);
                element = document.DocumentElement;
            }
            else
            {
                element = (Configuration.Instance.Settings["nhibernate"] as XmlNode) as XmlElement;
            }

            foreach (XmlNode childNode in element)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    settings.Add(childNode.Attributes["key"].Value, childNode.Attributes["value"].Value);
                }
            }

            return settings;
        }
    }
}