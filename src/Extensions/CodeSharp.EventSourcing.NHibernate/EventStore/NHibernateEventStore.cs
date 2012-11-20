//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate;
using NHibernate.Criterion;

namespace CodeSharp.EventSourcing.EventStore.NHibernate
{
    /// <summary>
    /// NHibernate implementation of event store.
    /// </summary>
    public class NHibernateEventStore : IEventStore
    {
        #region Private Variables

        private IJsonSerializer _jsonSerializer;
        private ITypeNameMappingProvider _typeNameMappingProvider;
        private ISourcableEventTypeProvider _aggregateSourcableEventMappingProvider;
        private ISessionFactory _sessionFactory;
        private ILogger _logger;

        #endregion

        #region Constructors

        public NHibernateEventStore(
            IJsonSerializer eventSerializer,
            ITypeNameMappingProvider typeNameMapper,
            ISourcableEventTypeProvider aggregateRootEventTypeProvider,
            ISessionFactory sessionFactory,
            ILoggerFactory loggerFactory)
        {
            _jsonSerializer = eventSerializer;
            _typeNameMappingProvider = typeNameMapper;
            _aggregateSourcableEventMappingProvider = aggregateRootEventTypeProvider;
            _sessionFactory = sessionFactory;
            _logger = loggerFactory.Create("EventSourcing.EventStore.NHibernateEventStore");
        }

        #endregion

        public virtual void StoreEvents(IEnumerable<SourcableEvent> evnts)
        {
            if (evnts.Count() == 0)
            {
                return;
            }

            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    _logger.Debug("NHibernate transaction began.");

                    try
                    {
                        PersistEvents(session, evnts);
                        transaction.Commit();
                        _logger.Debug("NHibernate transaction committed.");
                    }
                    catch (ConcurrencyException ex)
                    {
                        transaction.Rollback();
                        _logger.Info("NHibernate transaction rolled back.");
                        _logger.Error(ex);
                        throw;
                    }
                    catch (StaleObjectStateException ex)
                    {
                        transaction.Rollback();
                        _logger.Info("NHibernate transaction rolled back.");
                        _logger.Error(ConcurrencyException.DefaultConcurrencyExceptionMessage, ex);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.Info("NHibernate transaction rolled back.");
                        _logger.Error("Unknown error when storing sourcable events.", ex);
                        throw;
                    }
                }
            }
        }
        public virtual IEnumerable<SourcableEvent> GetEvents(string aggregateRootId, Type aggregateRootType, long minVersion, long maxVersion)
        {
            var evnts = new List<SourcableEvent>();
            var session = _sessionFactory.OpenSession();

            Type eventType = _aggregateSourcableEventMappingProvider.GetSourcableEventType(aggregateRootType);
            ICriteria criteria = session.CreateCriteria(eventType);
            criteria.Add(Restrictions.Eq("AggregateRootId", aggregateRootId));
            criteria.Add(Restrictions.Eq("AggregateRootName", GetAggregateRootName(aggregateRootType)));
            criteria.Add(Restrictions.Ge("Version", minVersion));
            criteria.Add(Restrictions.Le("Version", maxVersion));
            criteria.AddOrder(new Order("Version", true));

            var eventList = EventQueryHelper.GetEventList(eventType, criteria);

            foreach (var evnt in eventList)
            {
                var sourcableEvent = evnt as SourcableEvent;
                sourcableEvent.AggregateRootType = aggregateRootType;
                sourcableEvent.RawEvent = DeserializeEvent(GetEventType(sourcableEvent.Name), sourcableEvent.Data);
                evnts.Add(sourcableEvent);
            }

            return evnts;
        }

        protected virtual void PersistEvents(ISession session, IEnumerable<SourcableEvent> evnts)
        {
            //先对事件按照聚合根进行分组
            var eventGroupList = from evnt in evnts
                                 group evnt by new { AggregateRootType = evnt.AggregateRootType, evnt.AggregateRootId } into groupedEvents
                                 select groupedEvents;

            //持久化每个聚合根的事件
            foreach (var eventGroup in eventGroupList)
            {
                var aggregateRootType = eventGroup.Key.AggregateRootType;
                var firstEvent = eventGroup.First();
                var aggregateRootId = firstEvent.AggregateRootId;
                var firstEventVersion = firstEvent.Version;
                var lastEventVersion = eventGroup.Last().Version;
                var aggregateRootVersionObject = AggregateRootVersionHelper.GetAggregateRootVersion(session, aggregateRootType, aggregateRootId);

                if (aggregateRootVersionObject == null)
                {
                    AggregateRootVersionHelper.CreateAggregateRootVersion(session, aggregateRootType, aggregateRootId, lastEventVersion);
                    InsertSourcableEvents(session, aggregateRootType, eventGroup);
                }
                else if (aggregateRootVersionObject.Version == firstEventVersion - 1)
                {
                    aggregateRootVersionObject.Version = lastEventVersion;
                    session.Update(aggregateRootVersionObject);
                    InsertSourcableEvents(session, aggregateRootType, eventGroup);
                }
                else
                {
                    throw new ConcurrencyException(ConcurrencyException.DefaultConcurrencyExceptionMessage);
                }
            }
        }

        #region Private Methods

        private void InsertSourcableEvents(ISession session, Type aggregateRootType, IEnumerable<SourcableEvent> evnts)
        {
            var aggregateRootName = GetAggregateRootName(aggregateRootType);
            foreach (var evnt in evnts)
            {
                evnt.AggregateRootName = aggregateRootName;
                evnt.Name = GetEventName(evnt.RawEvent.GetType());
                evnt.Data = SerializeEvent(evnt.RawEvent);
                session.Save(evnt);

                string eventDetail = string.Format(
                    "AggregateRootName: {0}, aggregateRootId: {1}, Version: {2}, OccurredTime: {3}, EventName: {4}, EventData: {5}",
                    evnt.AggregateRootName,
                    evnt.AggregateRootId,
                    evnt.Version,
                    evnt.OccurredTime,
                    evnt.Name,
                    evnt.Data);
                _logger.DebugFormat("sourcable event inserted, detail:{0}", eventDetail);
            }
        }
        private Type GetAggregateRootType(string aggregateRootName)
        {
            return _typeNameMappingProvider.GetType(NameTypeMappingType.AggregateRootMapping, aggregateRootName);
        }
        private string GetAggregateRootName(Type aggregateRootType)
        {
            return _typeNameMappingProvider.GetName(NameTypeMappingType.AggregateRootMapping, aggregateRootType);
        }
        private Type GetEventType(string eventName)
        {
            return _typeNameMappingProvider.GetType(NameTypeMappingType.SourcableEventMapping, eventName);
        }
        private string GetEventName(Type eventType)
        {
            return _typeNameMappingProvider.GetName(NameTypeMappingType.SourcableEventMapping, eventType);
        }
        private string SerializeEvent(object evnt)
        {
            return _jsonSerializer.Serialize(evnt);
        }
        private object DeserializeEvent(Type eventType, string eventJson)
        {
            return _jsonSerializer.Deserialize(eventJson, eventType);
        }

        #endregion

        private class EventQueryHelper
        {
            private static MethodInfo _getEventListMethod = typeof(EventQueryHelper).GetMethod("GetEventList", BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
            private static EventQueryHelper _instance = new EventQueryHelper();

            public static IList GetEventList(Type sourcableEventType, ICriteria criteria)
            {
                return _getEventListMethod.MakeGenericMethod(sourcableEventType).Invoke(_instance, new object[] { criteria }) as IList;
            }

            private IList<TSourcableEvent> GetEventList<TSourcableEvent>(ICriteria criteria) where TSourcableEvent : SourcableEvent
            {
                return criteria.List<TSourcableEvent>();
            }
        }
        private class AggregateRootVersionHelper
        {
            private static MethodInfo _getAggregateRootVersionObjectMethod = typeof(AggregateRootVersionHelper).GetMethod("GetAggregateRootVersionObject", BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
            private static MethodInfo _createAggregateRootVersionObjectMethod = typeof(AggregateRootVersionHelper).GetMethod("CreateAggregateRootVersionObject", BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
            private static AggregateRootVersionHelper _instance = new AggregateRootVersionHelper();

            /// <summary>
            /// 返回指定聚合根的版本信息
            /// </summary>
            /// <param name="session"></param>
            /// <param name="aggregateRootType"></param>
            /// <param name="aggregateRootId"></param>
            /// <returns></returns>
            public static AggregateRootVersion GetAggregateRootVersion(ISession session, Type aggregateRootType, string aggregateRootId)
            {
                return _getAggregateRootVersionObjectMethod.MakeGenericMethod(aggregateRootType).Invoke(_instance, new object[] { session, aggregateRootId }) as AggregateRootVersion;
            }
            /// <summary>
            /// 创建聚合根的版本信息
            /// </summary>
            /// <param name="session"></param>
            /// <param name="aggregateRootType"></param>
            /// <param name="aggregateRootId"></param>
            /// <param name="version"></param>
            /// <returns></returns>
            public static void CreateAggregateRootVersion(ISession session, Type aggregateRootType, string aggregateRootId, long version)
            {
                _createAggregateRootVersionObjectMethod.MakeGenericMethod(aggregateRootType).Invoke(_instance, new object[] { session, aggregateRootId, version });
            }

            /// <summary>
            /// 返回指定聚合根的版本信息
            /// </summary>
            /// <typeparam name="TAggregateRoot"></typeparam>
            /// <param name="session"></param>
            /// <param name="aggregateRootId"></param>
            /// <returns></returns>
            private AggregateRootVersion<TAggregateRoot> GetAggregateRootVersionObject<TAggregateRoot>(ISession session, string aggregateRootId) where TAggregateRoot : AggregateRoot
            {
                var criteria = session.CreateCriteria<AggregateRootVersion<TAggregateRoot>>();
                criteria.Add(Expression.Eq("AggregateRootId", aggregateRootId));
                return criteria.UniqueResult<AggregateRootVersion<TAggregateRoot>>();
            }
            /// <summary>
            /// 创建聚合根的版本信息
            /// </summary>
            /// <typeparam name="TAggregateRoot"></typeparam>
            /// <param name="session"></param>
            /// <param name="aggregateRootId"></param>
            /// <param name="version"></param>
            /// <returns></returns>
            private void CreateAggregateRootVersionObject<TAggregateRoot>(ISession session, string aggregateRootId, long version) where TAggregateRoot : AggregateRoot
            {
                session.Save(new AggregateRootVersion<TAggregateRoot> { AggregateRootId = aggregateRootId, Version = version });
            }
        }
    }
}
