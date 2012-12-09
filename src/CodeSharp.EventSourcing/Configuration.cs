//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// EventSourcing框架核心配置类
    /// </summary>
    public class Configuration
    {
        private static Configuration _instance;
        private string _environment;

        /// <summary>
        /// Singleton单例
        /// </summary>
        public static Configuration Instance { get { return _instance; } }
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; private set; }
        /// <summary>
        /// 应用的所有配置信息
        /// </summary>
        public IDictionary<string, object> Settings { get; private set; }

        private Configuration(string appName)
        {
            if (string.IsNullOrEmpty(appName))
            {
                throw new ArgumentNullException("appName");
            }

            AppName = appName;
            Settings = new Dictionary<string, object>();
        }

        /// <summary>
        /// 采用所有默认方式配置EventSourcing框架。
        /// <remarks>
        /// 默认使用DefaultConfigurationInitializer以及TinyIoc Container来初始化框架。
        /// </remarks>
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <param name="configAssembly">配置文件资源所在程序集</param>
        /// <param name="assemblies">其他相关的程序集</param>
        /// <returns></returns>
        public static Configuration Config(string appName, Assembly configAssembly, params Assembly[] assemblies)
        {
            return Configuration.Config(appName, new DefaultConfigurationInitializer(configAssembly), assemblies);
        }
        /// <summary>
        /// 采用所有默认方式配置EventSourcing框架，配置信息初始化器由用户指定。
        /// <remarks>
        /// 用户可以调用此重载使用自己喜欢的方式来初始化框架配置信息。
        /// </remarks>
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <param name="configurationInitializer">框架配置初始化器</param>
        /// <param name="assemblies">其他相关的程序集</param>
        /// <returns></returns>
        public static Configuration Config(string appName, IConfigurationInitializer configurationInitializer, params Assembly[] assemblies)
        {
            return Configuration.Config(appName, configurationInitializer, new TinyIoCObjectContainer(), assemblies);
        }
        /// <summary>
        /// 采用所有默认方式配置EventSourcing框架，IoC容器由用户指定。
        /// <remarks>
        /// 用户可以调用此重载告诉框架该使用哪个IoC容器。
        /// </remarks>
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <param name="configAssembly">配置文件资源所在程序集</param>
        /// <param name="container">对象容器</param>
        /// <param name="assemblies">其他相关的程序集</param>
        /// <returns></returns>
        public static Configuration Config(string appName, Assembly configAssembly, IObjectContainer container, params Assembly[] assemblies)
        {
            return Configuration.Config(appName, new DefaultConfigurationInitializer(configAssembly), container, assemblies);
        }
        /// <summary>
        /// 采用所有默认方式配置EventSourcing框架，配置信息初始化器以及IoC容器都由用户指定。
        /// <remarks>
        /// 用户可以调用此重载使用自己喜欢的方式来初始化框架配置信息以及告诉框架该使用哪个IoC容器。
        /// </remarks>
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <param name="configurationInitializer">框架配置初始化器</param>
        /// <param name="container">对象容器</param>
        /// <param name="assemblies">其他相关的程序集</param>
        /// <returns></returns>
        public static Configuration Config(string appName, IConfigurationInitializer configurationInitializer, IObjectContainer container, params Assembly[] assemblies)
        {
            return Configuration.Create(appName).Initialize(configurationInitializer).Container(container, assemblies);
        }
        /// <summary>
        /// 创建一个配置类实例，不能重复调用该方法进行创建
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <returns></returns>
        public static Configuration Create(string appName)
        {
            if (_instance != null)
            {
                throw new EventSourcingException("不可重复初始化框架配置");
            }
            _instance = new Configuration(appName);
            return _instance;
        }
        /// <summary>
        /// 设置框架配置信息
        /// </summary>
        /// <param name="collection"></param>
        public void SetSettings(NameValueCollection collection)
        {
            foreach (string key in collection.Keys)
            {
                if (Settings.ContainsKey(key))
                {
                    Settings[key] = collection[key];
                }
                else
                {
                    Settings.Add(key, collection[key]);
                }
            }
        }
        /// <summary>
        /// 获取指定名称的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetSetting<T>(string name)
        {
            return Utils.ConvertType<T>(Settings[name]);
        }
        /// <summary>
        /// 设置当前运行环境，可能的值有：Debug,Test,Release，只能设置一次
        /// </summary>
        /// <param name="environment"></param>
        public Configuration SetEnvironment(string environment)
        {
            if (_environment != null)
            {
                throw new EventSourcingException("不能重复设置框架运行环境");
            }
            _environment = environment;
            return this;
        }
        /// <summary>
        /// 通过某个用户给定的IConfigurationInitializer来对当前的配置类实例进行初始化
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public Configuration Initialize(IConfigurationInitializer initializer)
        {
            initializer.Initialize(this);
            return this;
        }
        /// <summary>
        /// 设置对象容器并自动注册框架所有默认实现类并自动对给定程序集进行初始化
        /// </summary>
        public Configuration Container(IObjectContainer container, params Assembly[] assemblies)
        {
            ObjectContainer.SetContainer(container);

            //向容器中注册框架所有接口的默认实现
            ObjectContainer.Register<ILoggerFactory, Log4NetLoggerFactory>();
            ObjectContainer.Register<IAggregateRootTypeProvider, DefaultAggregateRootTypeProvider>();
            ObjectContainer.Register<ISnapshotTypeProvider, DefaultSnapshotTypeProvider>();
            ObjectContainer.Register<IAggregateRootFactory, DefaultAggregateRootFactory>();
            ObjectContainer.Register<ISerializer, JsonNetSerializer>(LifeStyle.Transient);
            ObjectContainer.Register<ISnapshotter, DefaultSnapshotter>(LifeStyle.Transient);
            ObjectContainer.Register<ISnapshotPolicy, NoSnapshotPolicy>(LifeStyle.Transient);
            ObjectContainer.Register<IMessageSerializer, DefaultMessageSerializer>(LifeStyle.Transient);
            ObjectContainer.Register<ISubscriptionStorage, DefaultSubscriptionStorage>();
            ObjectContainer.Register<IMessageTransport, MsmqMessageTransport>(LifeStyle.Transient);
            ObjectContainer.Register<IContextLifetimeManager, DynamicContextLifetimeManager>();
            ObjectContainer.Register<IContextManager, DefaultContextManager>(LifeStyle.Transient);
            ObjectContainer.Register<ISyncEventHandlerProvider, DefaultSyncEventHandlerProvider>();
            ObjectContainer.Register<IAsyncEventHandlerProvider, DefaultAsyncEventHandlerProvider>();
            ObjectContainer.Register<IAggregateEventHandlerProvider, DefaultAggregateEventHandlerProvider>();
            ObjectContainer.Register<ISourcableEventTypeProvider, DefaultSourcableEventTypeProvider>();
            ObjectContainer.Register<IAggregateRootInternalHandlerProvider, DefaultAggregateRootInternalHandlerProvider>();
            ObjectContainer.Register<IEventTypeProvider, DefaultEventTypeProvider>();
            ObjectContainer.Register<ITypeNameMappingProvider, DefaultTypeNameMappingProvider>();
            ObjectContainer.Register<IEventStore, DefaultEventStore>(LifeStyle.Transient);
            ObjectContainer.Register<ISnapshotStore, EmptySnapshotStore>(LifeStyle.Transient);
            ObjectContainer.Register<ISyncEventPublisher, DefaultSyncEventPublisher>(LifeStyle.Transient);
            ObjectContainer.Register<IAsyncEventPublisher, DefaultAsyncEventPublisher>(LifeStyle.Transient);
            ObjectContainer.Register<IEventSubscriberEndpoint, DefaultEventSubscriberEndpoint>();
            ObjectContainer.Register<IDbConnectionFactory, SqlConnectionFactory>(LifeStyle.Transient);
            ObjectContainer.Register<IAggregateRootVersionTableProvider, DefaultAggregateRootVersionTableProvider>();
            ObjectContainer.Register<ISourcableEventTableProvider, DefaultSourcableEventTableProvider>();
            ObjectContainer.Register<IContextTransactionLifetimeManager, DynamicContextTransactionLifetimeManager>();
            ObjectContainer.Register<IContextTransactionManager, SqlContextTransactionManager>(LifeStyle.Transient);
            ObjectContainer.Register<ICurrentDbTransactionProvider, SqlContextTransactionManager>(LifeStyle.Transient);

            //对给定程序集进行初始化
            this
            .RegisterServices(assemblies)
            .RegisterComponents(assemblies)
            .RegisterRepositories(assemblies)
            .RegisterAggregateRootTypes(assemblies)
            .RegisterSourcableEvents(assemblies)
            .RegisterSourcableEventMappings(assemblies)
            .RegisterEventSubscribers(assemblies)
            .RegisterTypeNameMappings(assemblies)
            .RegisterAggregateRootInternalEventHandlers(assemblies)
            .RegisterSyncEventHandlers(assemblies)
            .RegisterAsyncEventHandlers(assemblies)
            .RegisterAggregateEventHandlers(assemblies);

            return this;
        }
        /// <summary>
        /// 注册日志记录器工厂实现类
        /// </summary>
        public Configuration LoggerFactory<T>() where T : class, ILoggerFactory
        {
            ObjectContainer.RegisterDefault<ILoggerFactory, T>();
            return this;
        }
        /// <summary>
        /// 注册聚合根类型提供者实现类
        /// </summary>
        public Configuration AggregateRootTypeProvider<T>() where T : class, IAggregateRootTypeProvider
        {
            ObjectContainer.RegisterDefault<IAggregateRootTypeProvider, T>();
            return this;
        }
        /// <summary>
        /// 注册快照类型提供者实现类
        /// </summary>
        public Configuration SnapshotTypeProvider<T>() where T : class, ISnapshotTypeProvider
        {
            ObjectContainer.RegisterDefault<ISnapshotTypeProvider, T>();
            return this;
        }
        /// <summary>
        /// 注册聚合根工厂实现类
        /// </summary>
        public Configuration AggregateRootFactory<T>() where T : class, IAggregateRootFactory
        {
            ObjectContainer.RegisterDefault<IAggregateRootFactory, T>();
            return this;
        }
        /// <summary>
        /// 注册Serializer实现类
        /// </summary>
        public Configuration Serializer<T>() where T : class, ISerializer
        {
            ObjectContainer.RegisterDefault<ISerializer, T>(LifeStyle.Transient);
            return this;
        }
        /// <summary>
        /// 注册快照解析器实现类
        /// </summary>
        public Configuration Snapshotter<T>() where T : class, ISnapshotter
        {
            ObjectContainer.RegisterDefault<ISnapshotter, T>(LifeStyle.Transient);
            return this;
        }
        /// <summary>
        /// 注册快照创建策略实现类
        /// </summary>
        public Configuration SnapshotPolicy<T>() where T : class, ISnapshotPolicy
        {
            ObjectContainer.RegisterDefault<ISnapshotPolicy, T>(LifeStyle.Transient);
            return this;
        }
        /// <summary>
        /// 注册消息序列化器实现类
        /// </summary>
        public Configuration MessageSerializer<T>() where T : class, IMessageSerializer
        {
            ObjectContainer.RegisterDefault<IMessageSerializer, T>(LifeStyle.Transient);
            return this;
        }
        /// <summary>
        /// 注册事件订阅信息存储器实现类
        /// </summary>
        public Configuration SubscriptionStorage<T>() where T : class, ISubscriptionStorage
        {
            ObjectContainer.RegisterDefault<ISubscriptionStorage, T>();
            return this;
        }
        /// <summary>
        /// 注册MessageTransport实现类
        /// </summary>
        public Configuration MessageTransport<T>() where T : class, IMessageTransport
        {
            ObjectContainer.RegisterDefault<IMessageTransport, T>(LifeStyle.Transient);
            return this;
        }
        /// <summary>
        /// 注册Context的生命周期管理器实现类
        /// </summary>
        public Configuration ContextLifetimeManager<T>() where T : class, IContextLifetimeManager
        {
            ObjectContainer.RegisterDefault<IContextLifetimeManager, T>();
            return this;
        }
        /// <summary>
        /// 注册ContextManager实现类
        /// </summary>
        public Configuration ContextManager<T>() where T : class, IContextManager
        {
            ObjectContainer.RegisterDefault<IContextManager, T>(LifeStyle.Transient);
            return this;
        }
        /// <summary>
        /// 注册同步事件处理函数提供者实现类
        /// </summary>
        public Configuration SyncEventHandlerProvider<T>() where T : class, ISyncEventHandlerProvider
        {
            ObjectContainer.RegisterDefault<ISyncEventHandlerProvider, T>();
            return this;
        }
        /// <summary>
        /// 注册异步事件处理函数提供者实现类
        /// </summary>
        public Configuration AsyncEventHandlerProvider<T>() where T : class, IAsyncEventHandlerProvider
        {
            ObjectContainer.RegisterDefault<IAsyncEventHandlerProvider, T>();
            return this;
        }
        /// <summary>
        /// 注册聚合根事件处理函数提供者实现类
        /// </summary>
        public Configuration AggregateEventHandlerProvider<T>() where T : class, IAggregateEventHandlerProvider
        {
            ObjectContainer.RegisterDefault<IAggregateEventHandlerProvider, T>();
            return this;
        }
        /// <summary>
        /// 注册可溯源事件类型提供者实现类
        /// </summary>
        public Configuration SourcableEventTypeProvider<T>() where T : class, ISourcableEventTypeProvider
        {
            ObjectContainer.RegisterDefault<ISourcableEventTypeProvider, T>();
            return this;
        }
        /// <summary>
        /// 注册聚合根内部回调函数元数据提供者实现类
        /// </summary>
        public Configuration AggregateRootInternalHandlerProvider<T>() where T : class, IAggregateRootInternalHandlerProvider
        {
            ObjectContainer.RegisterDefault<IAggregateRootInternalHandlerProvider, T>();
            return this;
        }
        /// <summary>
        /// 注册类型与名称映射信息提供者实现类
        /// </summary>
        public Configuration TypeNameMappingProvider<T>() where T : class, ITypeNameMappingProvider
        {
            ObjectContainer.RegisterDefault<ITypeNameMappingProvider, T>();
            return this;
        }
        /// <summary>
        /// 注册事件类型提供者实现类
        /// </summary>
        public Configuration EventTypeProvider<T>() where T : class, IEventTypeProvider
        {
            ObjectContainer.RegisterDefault<IEventTypeProvider, T>();
            return this;
        }
        /// <summary>
        /// 注册EventStore实现类
        /// </summary>
        public Configuration EventStore<T>() where T : class, IEventStore
        {
            ObjectContainer.RegisterDefault<IEventStore, T>(LifeStyle.Transient);
            return this;
        }
        /// <summary>
        /// 注册SnapshotStore实现类
        /// </summary>
        public Configuration SnapshotStore<T>() where T : class, ISnapshotStore
        {
            ObjectContainer.RegisterDefault<ISnapshotStore, T>(LifeStyle.Transient);
            return this;
        }
        /// <summary>
        /// 注册同步的EventPublisher实现类
        /// </summary>
        public Configuration SyncEventPublisher<T>() where T : class, ISyncEventPublisher
        {
            ObjectContainer.RegisterDefault<ISyncEventPublisher, T>();
            return this;
        }
        /// <summary>
        /// 注册异步的EventPublisher实现类
        /// </summary>
        public Configuration AsyncEventPublisher<T>() where T : class, IAsyncEventPublisher
        {
            ObjectContainer.RegisterDefault<IAsyncEventPublisher, T>();
            return this;
        }
        /// <summary>
        /// 注册事件订阅者端点实现类
        /// </summary>
        public Configuration EventSubscriberEndpoint<T>() where T : class, IEventSubscriberEndpoint
        {
            ObjectContainer.RegisterDefault<IEventSubscriberEndpoint, T>();
            return this;
        }
        /// <summary>
        /// 注册数据库连接工厂实现类
        /// </summary>
        public Configuration DbConnectionFactory<T>() where T : class, IDbConnectionFactory
        {
            ObjectContainer.RegisterDefault<IDbConnectionFactory, T>();
            return this;
        }
        /// <summary>
        /// 注册聚合根的可溯源事件的表名提供者
        /// </summary>
        public Configuration SourcableEventTableProvider<T>() where T : class, ISourcableEventTableProvider
        {
            ObjectContainer.RegisterDefault<ISourcableEventTableProvider, T>();
            return this;
        }
        /// <summary>
        /// 注册聚合根的可溯源事件版本信息的表名提供者
        /// </summary>
        public Configuration AggregateRootVersionTableProvider<T>() where T : class, IAggregateRootVersionTableProvider
        {
            ObjectContainer.RegisterDefault<IAggregateRootVersionTableProvider, T>();
            return this;
        }
        /// <summary>
        /// 注册Context事务管理器实现类
        /// </summary>
        public Configuration ContextTransactionManager<T>() where T : class, IContextTransactionManager
        {
            ObjectContainer.RegisterDefault<IContextTransactionManager, T>();
            return this;
        }
        /// <summary>
        /// 注册数据库事务管理器实现类
        /// </summary>
        public Configuration DbTransactionManager<T>() where T : class, ICurrentDbTransactionProvider
        {
            ObjectContainer.RegisterDefault<ICurrentDbTransactionProvider, T>();
            return this;
        }

        /// <summary>
        /// 注册给定程序集中所有的组件
        /// </summary>
        public Configuration RegisterComponents(params Assembly[] assemblies)
        {
            ObjectContainer.RegisterTypes(TypeUtils.IsComponent, assemblies);
            return this;
        }
        /// <summary>
        /// 注册给定程序集中所有的仓储
        /// </summary>
        public Configuration RegisterRepositories(params Assembly[] assemblies)
        {
            ObjectContainer.RegisterTypes(TypeUtils.IsRepository, assemblies);
            return this;
        }
        /// <summary>
        /// 注册给定程序集中所有的服务
        /// </summary>
        public Configuration RegisterServices(params Assembly[] assemblies)
        {
            ObjectContainer.RegisterTypes(TypeUtils.IsService, assemblies);
            return this;
        }
        /// <summary>
        /// 注册给定程序集中所有的聚合根的类型
        /// </summary>
        public Configuration RegisterAggregateRootTypes(params Assembly[] assemblies)
        {
            ObjectContainer.Resolve<IAggregateRootTypeProvider>().RegisterAggregateRootTypes(assemblies);
            return this;
        }
        /// <summary>
        /// 注册给定程序集中所有的可溯源事件
        /// </summary>
        public Configuration RegisterSourcableEvents(params Assembly[] assemblies)
        {
            ObjectContainer.RegisterTypes(TypeUtils.IsSourcableEvent, assemblies);
            return this;
        }
        /// <summary>
        /// 注册给定程序集中所有的可溯源事件与其对应聚合根的映射关系
        /// </summary>
        public Configuration RegisterSourcableEventMappings(params Assembly[] assemblies)
        {
            ObjectContainer.Resolve<ISourcableEventTypeProvider>().RegisterMappings(assemblies);
            return this;
        }
        /// <summary>
        /// 注册给定程序集中所有的事件订阅者
        /// </summary>
        public Configuration RegisterEventSubscribers(params Assembly[] assemblies)
        {
            ObjectContainer.RegisterTypes(TypeUtils.IsEventSubscriber, assemblies);
            return this;
        }
        /// <summary>
        /// 注册给定程序集中所有聚合根、可溯源事件，以及快照的类型与其名称的映射关系
        /// </summary>
        public Configuration RegisterTypeNameMappings(params Assembly[] assemblies)
        {
            var provider = ObjectContainer.Resolve<ITypeNameMappingProvider>();
            provider.RegisterMappings(NameTypeMappingType.AggregateRootMapping, assemblies);
            provider.RegisterMappings(NameTypeMappingType.SourcableEventMapping, assemblies);
            provider.RegisterMappings(NameTypeMappingType.SnapshotMapping, assemblies);
            return this;
        }
        /// <summary>
        /// 注册给定程序集中所有的聚合根内部回调函数
        /// </summary>
        public Configuration RegisterAggregateRootInternalEventHandlers(params Assembly[] assemblies)
        {
            ObjectContainer.Resolve<IAggregateRootInternalHandlerProvider>().RegisterInternalHandlers(assemblies);
            return this;
        }
        /// <summary>
        /// 注册给定程序集中所有的同步事件处理函数
        /// </summary>
        public Configuration RegisterSyncEventHandlers(params Assembly[] assemblies)
        {
            ObjectContainer.Resolve<ISyncEventHandlerProvider>().RegisterEventSubscribers(assemblies);
            return this;
        }
        /// <summary>
        /// 注册给定程序集中所有的异步事件处理函数
        /// </summary>
        public Configuration RegisterAsyncEventHandlers(params Assembly[] assemblies)
        {
            ObjectContainer.Resolve<IAsyncEventHandlerProvider>().RegisterEventSubscribers(assemblies);
            return this;
        }
        /// <summary>
        /// 注册给定程序集中所有的聚合根事件处理函数
        /// </summary>
        public Configuration RegisterAggregateEventHandlers(params Assembly[] assemblies)
        {
            ObjectContainer.Resolve<IAggregateEventHandlerProvider>().RegisterEventSubscribers(assemblies);
            return this;
        }
        /// <summary>
        /// 启动事件订阅者端点
        /// </summary>
        public Configuration StartEventSubscriberEndpoint(string endpointAddress = null, bool clearSubscriptions = true)
        {
            var endpoint = ObjectContainer.Resolve<IEventSubscriberEndpoint>();
            endpoint.Initialize(endpointAddress ?? Configuration.Instance.AppName, clearSubscriptions);
            endpoint.Start();
            return this;
        }
    }
}