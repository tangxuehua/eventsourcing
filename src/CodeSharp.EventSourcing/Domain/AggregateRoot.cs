//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// DDD聚合根抽象基类
    /// </summary>
    public abstract class AggregateRoot
    {
        #region Private Variables

        private string _uniqueId;
        private Queue<SourcableEvent> _sourcableEvents;
        private Queue<object> _notifyEvents;
        private long _originalVersion;
        private const long DefaultVersion = 0;
        private ILogger _logger;
        private IAggregateRootInternalHandlerProvider _callbackProvider;
        private IEventTypeProvider _eventTypeProvider;

        #endregion

        #region Constructurs

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AggregateRoot()
        {
            _sourcableEvents = new Queue<SourcableEvent>();
            _notifyEvents = new Queue<object>();
            _originalVersion = DefaultVersion;
            _logger = ObjectContainer.Resolve<ILoggerFactory>().Create("EventSourcing.AggregateRoot");
            _callbackProvider = ObjectContainer.Resolve<IAggregateRootInternalHandlerProvider>();
            _eventTypeProvider = ObjectContainer.Resolve<IEventTypeProvider>();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="uniqueId">聚合根唯一标识</param>
        public AggregateRoot(string uniqueId) : this()
        {
            _uniqueId = uniqueId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 聚合根的非泛型唯一标识
        /// </summary>
        public string UniqueId
        {
            get
            {
                return _uniqueId;
            }
            protected set
            {
                _uniqueId = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 将聚合根转换为指定的角色，角色必须是一个接口。
        /// <remarks>
        /// 注：要转换的聚合根必须实现该角色接口。
        /// </remarks>
        /// </summary>
        /// <typeparam name="TRole">要转换的角色接口</typeparam>
        /// <returns>返回转换后的角色实例，实际就是扮演了该角色的当前聚合根实例。</returns>
        public TRole ActAs<TRole>() where TRole : class
        {
            if (!typeof(TRole).IsInterface)
            {
                throw new EventSourcingException(string.Format("TRole '{0}' must be an interface.", typeof(TRole).FullName));
            }

            var role = this as TRole;

            if (role == null)
            {
                throw new EventSourcingException(string.Format("AggregateRoot '{0}' can not act as role '{1}'.", this.GetType().FullName, typeof(TRole).FullName));
            }

            return role;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// 通知框架聚合根内某个事件发生了。
        /// <remarks>
        /// 需要注意的是，这里的事件不是Domain Event，而是System Event。技术开发人员经常把Domain Event与System Event混淆。
        /// 下面是关于这两者的区别的一段描述：
        /// Domain events has nothing to do with system events, event sourcing, any technical patterns or architectural pattern like CQRS.
        /// Domain events are something that actually happens in your customers business, not in your software. 
        /// Below is an excerpt（摘录，引用） from the domain event pattern, as defined by Eric Evans.
        /// Model information about activity in the domain as a series of discrete（不相关联的，非连续的） events.
        /// Represent each event as a domain object. These are distinct from system events that reflect activity within the software itself,
        /// although often a system event is associated with a domain event,
        /// either as part of a response to the domain event or as a way of carrying information about the domain event into the system.
        /// A domain event is a full-fledged（羽翼丰满的） part of the domain model, a representation of something that happened in the domain.
        /// Ignore irrelevant（不相干的，不恰当的） domain activity while making explicit the events that the domain experts want to track or be notified of,
        /// or which are associated with state change in the other model objects. – Eric Evans
        /// </remarks>
        /// </summary>
        /// <param name="evnt">已发生的事件对象</param>
        protected void OnEventHappened(object evnt)
        {
            if (evnt == null)
            {
                throw new ArgumentNullException("evnt");
            }

            AssertCurrentAggregateRoot();

            if (_eventTypeProvider.IsSourcableEvent(evnt.GetType()))
            {
                HandleEvent(evnt);
                AppendSourcableEvent(CreateSourcableEvent(evnt, GetNextVersion()));
                _logger.DebugFormat("Sourcable event happened:{0}", evnt.GetType().FullName);
            }
            else if (_eventTypeProvider.IsNotityEvent(evnt.GetType()))
            {
                AppendNotifyEvent(evnt);
                _logger.DebugFormat("Notify event happened:{0}", evnt.GetType().FullName);
            }
            else
            {
                throw new EventSourcingException("Unknown event '{0}'", evnt.GetType().FullName);
            }
        }
        /// <summary>
        /// 该方法返回一个SourcableEvent对象，当用户扩展了SourcableEvent类时，可以重载此方法返回一个自己的SourcableEvent对象。
        /// 或者更加对推荐的方式是，用户可以设计一个实现了ISourcableEvent的泛型接口SourcableEvent类来实现对SourcableEvent的扩展。
        /// </summary>
        /// <param name="evnt">用户创建的事件对象</param>
        /// <returns>返回一个SourcableEvent对象</returns>
        protected virtual SourcableEvent CreateSourcableEvent(object evnt, long version)
        {
            var sourcableEventInterfaceType = typeof(ISourcableEvent<>).MakeGenericType(GetType());
            if (ObjectContainer.IsRegistered(sourcableEventInterfaceType))
            {
                return MethodInvoker.InvokeMethod(sourcableEventInterfaceType, "Initialize", new object[] { this, evnt, version }) as SourcableEvent;
            }
            else
            {
                var sourcableEventType = typeof(SourcableEvent<>).MakeGenericType(GetType());
                return MethodInvoker.InvokeMethod(sourcableEventType, "Initialize", new object[] { this, evnt, version }) as SourcableEvent;
            }
        }
        /// <summary>
        /// 通知框架一个新的聚合根被创建了，框架会将该新建的聚合根自动添加到当前Context。
        /// </summary>
        /// <param name="instance">新建的聚合根对象</param>
        protected void OnAggregateRootCreated(AggregateRoot instance)
        {
            AssertCurrentAggregateRoot();
            using (var context = ObjectContainer.Resolve<IContextManager>().GetContext())
            {
                context.Add(instance);
            }
        }
        /// <summary>
        /// 在领域中唤醒指定ID的聚合根并返回该聚合根。
        /// <remarks>
        /// 该方法的使用背景：
        /// 在应用EventSourcing的架构下，我们一般通过ID来关联聚合根；正因为这个原因导致如果我们要从
        /// 一个聚合根想去导航到另一个聚合根实现某种交互时，必须通过某种容易理解的措施获取ID所对应的聚合根的引用；
        /// 出于这个目的，我们在框架中提供此方法可以让当前聚合根能够方便地在领域中“唤醒”某个指定的聚合根。
        /// </remarks>
        /// </summary>
        /// <param name="id">要唤醒的聚合根的ID</param>
        protected T WakeupAggregateRoot<T>(object id) where T : AggregateRoot
        {
            AssertCurrentAggregateRoot();
            using (var context = ObjectContainer.Resolve<IContextManager>().GetContext())
            {
                return context.Load<T>(id);
            }
        }
        /// <summary>
        /// 自动根据事件更新聚合根的状态。
        /// <remarks>
        /// 基于约定来更新，约定规则是：
        /// 针对事件和当前聚合根中BindingFlags为Public | Instance | DelcaredOnly的所有属性，则将事件中的每个
        /// 属性的值自动更新到当前聚合根上的对应属性。
        /// </remarks>
        /// </summary>
        protected virtual void UpdateAggregateRootAutomatically(object evnt)
        {
            if (evnt != null)
            {
                var bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                var propertiesOfEvent = evnt.GetType().GetProperties(bindingFlags);
                var propertiesOfAggregateRoot = this.GetType().GetProperties(bindingFlags);
                foreach (var property in propertiesOfEvent)
                {
                    var targetProperty = propertiesOfAggregateRoot.SingleOrDefault(x => x.Name == property.Name && x.PropertyType == property.PropertyType);
                    if (targetProperty != null)
                    {
                        targetProperty.SetValue(this, property.GetValue(evnt, null), null);
                    }
                }
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// 返回聚合根的原始版本号，原始版本号是指聚合根被创建或重建时的初始版本号；
        /// 该版本号不会随着聚合根在内存中发生的变化而变化；该版本号也与数据持久化无关；
        /// </summary>
        internal long GetOriginalVersion()
        {
            return _originalVersion;
        }
        /// <summary>
        /// 将当前聚合根上发生的可溯源事件弹出并返回
        /// </summary>
        internal IEnumerable<SourcableEvent> PopSourcableEvents()
        {
            var evnts = new List<SourcableEvent>();
            while (_sourcableEvents.Count() > 0)
            {
                evnts.Add(_sourcableEvents.Dequeue());
            }
            return evnts.AsReadOnly();
        }
        /// <summary>
        /// 将当前聚合根上发生的通知事件弹出并返回
        /// </summary>
        internal IEnumerable<object> PopNotifyEvents()
        {
            var evnts = new List<object>();
            while (_notifyEvents.Count() > 0)
            {
                evnts.Add(_notifyEvents.Dequeue());
            }
            return evnts.AsReadOnly();
        }
        /// <summary>
        /// 对给定的可溯源事件进行重演。
        /// <remarks>
        /// 注：重演之前如果当前已经存在未保存过的事件，则会被清除，原因是：
        /// 当框架通过事件溯源重建聚合根时，会调用聚合根的无参构造函数进行实例化，
        /// 而此时如果用户在无参构造函数中调用了OnEventHappened，那么框架在实例化好该聚合根准备调用
        /// 此ReplaySourcableEvents方法时，当前聚合根内就会有已经存在的未保存的事件。
        /// 而这些事件实际上是没有意义的，所以这里总是需要先清除事件再用给定的事件进行重演。
        /// </remarks>
        /// </summary>
        internal void ReplaySourcableEvents(IEnumerable<SourcableEvent> sourcableEvents)
        {
            if (_sourcableEvents.Count() > 0)
            {
                _sourcableEvents.Clear();
            }
            foreach (var sourcableEvent in sourcableEvents)
            {
                //首先验证事件的有效性
                VerifySourcableEvent(_originalVersion, sourcableEvent);
                //如果是第一个事件，则需要设置聚合根的唯一标识
                if (_originalVersion == DefaultVersion && sourcableEvent.Version == DefaultVersion + 1)
                {
                    _uniqueId = sourcableEvent.AggregateRootId;
                }
                //聚合根内部自己处理当前事件
                HandleEvent(sourcableEvent.RawEvent);
                //更新原始版本号
                _originalVersion = sourcableEvent.Version;
            }
        }
        /// <summary>
        /// 从给定的快照初始化当前聚合根
        /// </summary>
        internal void InitializeFromSnapshot(Snapshot snapshot)
        {
            _uniqueId = snapshot.AggregateRootId;
            _originalVersion = snapshot.Version;
            _sourcableEvents = new Queue<SourcableEvent>();
            _notifyEvents = new Queue<object>();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 调用聚合根内部事件响应函数响应事件，更新聚合根的内部状态
        /// </summary>
        private void HandleEvent(object evnt)
        {
            var callback = _callbackProvider.GetInternalEventHandler(GetType(), evnt.GetType());
            if (callback != null)
            {
                callback(this, evnt);
            }
            else
            {
                UpdateAggregateRootAutomatically(evnt);
            }
        }
        /// <summary>
        /// 事件重演时，验证某个给定的事件是否有效
        /// </summary>
        private void VerifySourcableEvent(long currentVersion, SourcableEvent sourcableEvent)
        {
            if (sourcableEvent.Version <= DefaultVersion)
            {
                throw new EventSourcingException(string.Format("事件的版本号无效，必须大于等于{0}", DefaultVersion + 1));
            }

            if (currentVersion == DefaultVersion)
            {
                if (sourcableEvent.Version != DefaultVersion + 1)
                {
                    throw new EventSourcingException(string.Format("应用到聚合根上的第一个事件的版本必须为{0}.", DefaultVersion + 1));
                }
            }
            else
            {
                if (sourcableEvent.AggregateRootId != _uniqueId)
                {
                    var message = string.Format("不允许将其他聚合根（aggregateRootId:{0}）的事件（详细信息:{1}）应用到当前聚合根（aggregateRootId:{2}）.",
                                                sourcableEvent.AggregateRootId, sourcableEvent.ToString(), _uniqueId);
                    throw new EventSourcingException(message);
                }
                if (sourcableEvent.Version != currentVersion + 1)
                {
                    var message = string.Format("不允许将版本为{0}事件应用到聚合根（aggregateRootId:{1}）. 因为该聚合根的当前版本是{2}, 只有版本为{3}的事件才可以被应用到该聚合根.",
                                                sourcableEvent.Version, _uniqueId, currentVersion, currentVersion + 1);
                    throw new EventSourcingException(message);
                }
            }
        }
        /// <summary>
        /// 追加一个新的溯源事件到队列
        /// </summary>
        private void AppendSourcableEvent(SourcableEvent sourcableEvent)
        {
            if (_sourcableEvents == null)
            {
                _sourcableEvents = new Queue<SourcableEvent>();
            }
            _sourcableEvents.Enqueue(sourcableEvent);
        }
        /// <summary>
        /// 追加一个新的通知事件到队列
        /// </summary>
        private void AppendNotifyEvent(object notifyEvent)
        {
            if (_notifyEvents == null)
            {
                _notifyEvents = new Queue<object>();
            }
            _notifyEvents.Enqueue(notifyEvent);
        }
        /// <summary>
        ///  验证当前聚合根是否是一个有效的聚合根
        /// <remarks>
        /// 聚合根的ID不能为空
        /// </remarks>
        /// </summary>
        private void AssertCurrentAggregateRoot()
        {
            if (string.IsNullOrEmpty(_uniqueId))
            {
                throw new EventSourcingException(string.Format("聚合根（Type:{0}）的Id为空，请确认是否调用了基类的需要传入聚合根Id的构造函数。", GetType().FullName));
            }
        }
        /// <summary>
        /// 获取下一个事件的版本号
        /// </summary>
        /// <returns></returns>
        private long GetNextVersion()
        {
            return _originalVersion + _sourcableEvents.Count + 1;
        }

        #endregion
    }
    /// <summary>
    /// 具有泛型ID的DDD聚合根抽象基类，继承自AggregateRoot抽象类
    /// </summary>
    public abstract class AggregateRoot<TAggregateRootId> : AggregateRoot
    {
        public AggregateRoot() : base()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">聚合根的Id</param>
        public AggregateRoot(TAggregateRootId id) : base(id.ToString())
        {
        }

        /// <summary>
        /// 聚合根的泛型唯一标识
        /// </summary>
        public TAggregateRootId Id
        {
            get
            {
                if (UniqueId != null)
                {
                    return Utils.ConvertType<TAggregateRootId>(UniqueId);
                }
                return default(TAggregateRootId);
            }
            set
            {
                base.UniqueId = Utils.ConvertType<string>(value);
            }
        }
    }
}
