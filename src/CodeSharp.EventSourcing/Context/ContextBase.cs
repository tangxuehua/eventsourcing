//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CodeSharp.EventSourcing
{
    public abstract class ContextBase : IContext
    {
        protected const int MaxTryCount = 100;
        protected List<AggregateRoot> _trackingAggregateRoots;
        protected ISyncEventPublisher _syncEventPublisher;
        protected IAsyncEventPublisher _asyncEventPublisher;
        protected IAggregateRootFactory _aggregateRootFactory;
        protected IAggregateEventHandlerProvider _aggregateEventHandlerProvider;
        protected IEventStore _eventStore;
        protected ISnapshotStore _snapshotStore;
        protected IContextTransactionManager _transactionManager;
        protected ILogger _logger;

        public abstract bool IsChildContext { get; }
        public Stack OwnerStack { get; set; }

        public ContextBase()
        {
            _trackingAggregateRoots = new List<AggregateRoot>();
            _syncEventPublisher = ObjectContainer.Resolve<ISyncEventPublisher>();
            _asyncEventPublisher = ObjectContainer.Resolve<IAsyncEventPublisher>();
            _aggregateRootFactory = ObjectContainer.Resolve<IAggregateRootFactory>();
            _aggregateEventHandlerProvider = ObjectContainer.Resolve<IAggregateEventHandlerProvider>();
            _eventStore = ObjectContainer.Resolve<IEventStore>();
            _snapshotStore = ObjectContainer.Resolve<ISnapshotStore>();
            _transactionManager = ObjectContainer.Resolve<IContextTransactionManager>();
            _logger = ObjectContainer.Resolve<ILoggerFactory>().Create(string.Format("EventSourcing.{0}", GetType().Name));
        }

        public virtual void Add(AggregateRoot instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            var existingAggregateRoot = _trackingAggregateRoots.SingleOrDefault(x => x.GetType() == instance.GetType() && x.UniqueId == instance.UniqueId);
            if (existingAggregateRoot != null)
            {
                throw new EventSourcingException("Aggregate root with the same id cannot be added in the current context, as there already an aggregate root with the same uniqueId exist. aggregate root id:{0}", instance.UniqueId);
            }

            _trackingAggregateRoots.Add(instance);
            _logger.DebugFormat("Aggregate root was tracked in context, type:{0}, id:{1}", instance.GetType().FullName, instance.UniqueId);
        }
        public virtual T Load<T>(object id) where T : AggregateRoot
        {
            return Load(typeof(T), id) as T;
        }
        public virtual AggregateRoot Load(Type aggregateRootType, object id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            var aggregateRootId = id.ToString();

            var existingAggregateRoot = _trackingAggregateRoots.SingleOrDefault(x => x.GetType() == aggregateRootType && x.UniqueId == aggregateRootId);
            if (existingAggregateRoot != null)
            {
                return existingAggregateRoot;
            }

            var aggregateRoot = GetFromStorage(aggregateRootType, aggregateRootId);

            if (aggregateRoot != null)
            {
                _trackingAggregateRoots.Add(aggregateRoot);
                _logger.DebugFormat("Aggregate root was tracked in context, type:{0}, id:{1}", aggregateRoot.GetType().FullName, aggregateRoot.UniqueId);
            }

            return aggregateRoot;
        }
        public virtual void SaveChanges()
        {
            IEnumerable<object> evnts = null;

            using (var transaction = _transactionManager.OpenContextTransaction())
            {
                try
                {
                    var sourcableEvents = FetchSourcableEvents();
                    if (sourcableEvents.Count() > 0)
                    {
                        var totalSourcableEvents = PublishSourcableEventsToDomain(sourcableEvents);
                        _eventStore.StoreEvents(totalSourcableEvents);
                        evnts = totalSourcableEvents.Select(evnt => evnt.RawEvent);
                        _syncEventPublisher.PublishEvents(evnts);
                    }

                    var notifyEvents = FetchNotifyEvents();
                    if (notifyEvents.Count() > 0)
                    {
                        _syncEventPublisher.PublishEvents(notifyEvents);
                    }

                    transaction.Commit();
                }
                catch (ConcurrencyException ex)
                {
                    transaction.Rollback();
                    _logger.Error(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.Error("Unknown error when saving context changes.", ex);
                    throw;
                }
            }

            if (evnts != null && evnts.Count() > 0)
            {
                _asyncEventPublisher.PublishEvents(evnts);
            }
        }
        public virtual void Dispose()
        {
        }

        #region Private Methods

        /// <summary>
        /// 从存储设备中查找一个聚合根
        /// </summary>
        private AggregateRoot GetFromStorage(Type aggregateRootType, string aggregateRootId)
        {
            AggregateRoot aggregateRoot = null;
            long minEventVersion = long.MinValue;
            long maxEventVersion = long.MaxValue;
            bool snapshotEnabled = Configuration.Instance.GetSetting<bool>("snapshotEnabled");

            if (snapshotEnabled && TryGetFromSnapshot(aggregateRootId, aggregateRootType, out aggregateRoot))
            {
                return aggregateRoot;
            }
            else
            {
                var allSourcableEvents = _eventStore.GetEvents(aggregateRootId, aggregateRootType, minEventVersion, maxEventVersion);
                aggregateRoot = BuildAggregateRoot(aggregateRootType, allSourcableEvents);
            }

            return aggregateRoot;
        }
        /// <summary>
        /// 尝试从快照获取聚合根
        /// </summary>
        private bool TryGetFromSnapshot(string aggregateRootId, Type aggregateRootType, out AggregateRoot aggregateRoot)
        {
            aggregateRoot = null;

            var snapshot = _snapshotStore.GetLastestSnapshot(aggregateRootId, aggregateRootType);
            if (snapshot != null && snapshot.IsValid())
            {
                AggregateRoot aggregateRootFromSnapshot = ObjectContainer.Resolve<ISnapshotter>().RestoreFromSnapshot(snapshot);
                if (aggregateRootFromSnapshot != null)
                {
                    if (aggregateRootFromSnapshot.UniqueId != aggregateRootId)
                    {
                        string message = string.Format("从快照还原出来的聚合根的Id({0})与所要求的Id({1})不符", aggregateRootFromSnapshot.UniqueId, aggregateRootId);
                        throw new EventSourcingException(message);
                    }
                    var committedEventsAfterSnapshot = _eventStore.GetEvents(aggregateRootId, aggregateRootType, snapshot.Version + 1, long.MaxValue);
                    aggregateRootFromSnapshot.ReplaySourcableEvents(committedEventsAfterSnapshot);
                    aggregateRoot = aggregateRootFromSnapshot;
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// 通过事件溯源的方式重建聚合根
        /// </summary>
        private AggregateRoot BuildAggregateRoot(Type aggregateRootType, IEnumerable<SourcableEvent> evnts)
        {
            AggregateRoot aggregateRoot = null;

            if (evnts != null && evnts.Count() > 0)
            {
                aggregateRoot = _aggregateRootFactory.CreateAggregateRoot(aggregateRootType);
                aggregateRoot.ReplaySourcableEvents(evnts);
            }

            return aggregateRoot;
        }
        /// <summary>
        /// 通过EventPublisher将指定的可溯源事件发回给领域，返回领域中当前上下文总共发生的可溯源事件；
        /// 之所以要发回给领域是因为领域中可能会有一些聚合根会响应这些事件。
        /// </summary>
        /// <param name="sourcableEvents"></param>
        /// <returns></returns>
        private IEnumerable<SourcableEvent> PublishSourcableEventsToDomain(IEnumerable<SourcableEvent> sourcableEvents)
        {
            _logger.Debug("Publishing sourcable events back to domain.");
            var totalSourcableEvents = new List<SourcableEvent>();
            RecursivelyPublishSourcableEventsToDomain(totalSourcableEvents, sourcableEvents, 1);
            _logger.Debug("Published sourcable events back to domain.");
            return totalSourcableEvents;
        }
        /// <summary>
        /// 以递归的方式将指定的事件发回给领域
        /// </summary>
        private void RecursivelyPublishSourcableEventsToDomain(List<SourcableEvent> totalSourcableEvents, IEnumerable<SourcableEvent> sourcableEvents, int triedCount)
        {
            if (sourcableEvents.Count() > 0)
            {
                if (triedCount <= MaxTryCount)
                {
                    foreach (var evnt in sourcableEvents.Select(x => x.RawEvent))
                    {
                        CallAggregateRootEventHandlers(evnt);
                    }
                    totalSourcableEvents.AddRange(sourcableEvents);
                    RecursivelyPublishSourcableEventsToDomain(totalSourcableEvents, FetchSourcableEvents(), triedCount + 1);
                }
                else
                {
                    throw new EventSourcingException("RecursivelyPublishSourcableEventsToDomain递归调用次数超过最大次数{0}，请检查领域模型内是否有循环事件依赖的问题。", MaxTryCount);
                }
            }
        }
        /// <summary>
        /// 返回当前被跟踪的所有聚合根上所发生的所有通知事件
        /// </summary>
        private IEnumerable<object> FetchNotifyEvents()
        {
            var evnts = new List<object>();
            _trackingAggregateRoots.ForEach(x => evnts.AddRange(x.PopNotifyEvents()));
            return evnts.AsReadOnly();
        }
        /// <summary>
        /// 返回当前被跟踪的所有聚合根上所发生的所有可溯源事件
        /// </summary>
        private IEnumerable<SourcableEvent> FetchSourcableEvents()
        {
            var evnts = new List<SourcableEvent>();
            _trackingAggregateRoots.ForEach(x => evnts.AddRange(x.PopSourcableEvents()));
            return evnts.AsReadOnly();
        }
        /// <summary>
        /// 调用领域模型内相关聚合根对某个指定事件的响应函数。
        /// </summary>
        private void CallAggregateRootEventHandlers(object evnt)
        {
            var eventHandlers = _aggregateEventHandlerProvider.GetEventHandlers(evnt.GetType());
            foreach (var eventHandler in eventHandlers)
            {
                if (eventHandler.Paths.Count() == 1)
                {
                    SourcableEventAggregateHandlerCaller.CallEventHandler(
                        eventHandler.SubscriberType,
                        eventHandler.EventHandler,
                        evnt,
                        eventHandler.Paths.Single().PropertyName);
                }
                else
                {
                    SourcableEventAggregateHandlerCaller.CallEventHandler(
                        eventHandler.EventHandler,
                        evnt,
                        eventHandler.Paths);
                }
            }
        }

        #endregion
    }
}
