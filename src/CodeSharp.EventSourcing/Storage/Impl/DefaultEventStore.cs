//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// Default implementation of event store, use ADO.NET to persist sourcable events.
    /// </summary>
    public class DefaultEventStore : IEventStore
    {
        #region Private Variables

        private ISerializer _serializer;
        private ITypeNameMappingProvider _typeNameMappingProvider;
        private ISourcableEventTypeProvider _sourcableEventTypeProvider;
        private ISourcableEventTableProvider _sourcableEventTableProvider;
        private IAggregateRootVersionTableProvider _aggregateRootVersionTableProvider;
        private ICurrentDbTransactionProvider _transactionProvider;
        private IDbConnectionFactory _connectionFactory;
        private ILogger _logger;

        #endregion

        #region Constructors

        public DefaultEventStore(
            ISerializer serializer,
            ITypeNameMappingProvider typeNameMappingProvider,
            ISourcableEventTypeProvider sourcableEventTypeProvider,
            ISourcableEventTableProvider sourcableEventTableProvider,
            IAggregateRootVersionTableProvider aggregateRootVersionTableProvider,
            ICurrentDbTransactionProvider transactionProvider,
            IDbConnectionFactory connectionFactory,
            ILoggerFactory loggerFactory)
        {
            _serializer = serializer;
            _typeNameMappingProvider = typeNameMappingProvider;
            _sourcableEventTypeProvider = sourcableEventTypeProvider;
            _sourcableEventTableProvider = sourcableEventTableProvider;
            _aggregateRootVersionTableProvider = aggregateRootVersionTableProvider;
            _transactionProvider = transactionProvider;
            _connectionFactory = connectionFactory;
            _logger = loggerFactory.Create("EventSourcing.DefaultEventStore");
        }

        #endregion

        public virtual void StoreEvents(IEnumerable<SourcableEvent> evnts)
        {
            if (evnts.Count() == 0)
            {
                return;
            }

            var transaction = _transactionProvider.CurrentTransaction;
            var connection = transaction.Connection;

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
                var table = _aggregateRootVersionTableProvider.GetTable(aggregateRootType);

                var aggregateRootVersionObject = connection.Query(new { AggregateRootId = aggregateRootId }, table, transaction).SingleOrDefault();

                if (aggregateRootVersionObject == null)
                {
                    connection.Insert(new { AggregateRootId = aggregateRootId, Version = lastEventVersion }, table, transaction);
                    InsertSourcableEvents(connection, aggregateRootType, eventGroup, transaction);
                }
                else if (aggregateRootVersionObject.Version == firstEventVersion - 1)
                {
                    var effectedRows = connection.Update(new { Version = lastEventVersion }, new { AggregateRootId = aggregateRootId }, table, transaction);
                    if (effectedRows != 1)
                    {
                        throw new ConcurrencyException(ConcurrencyException.DefaultConcurrencyExceptionMessage);
                    }
                    InsertSourcableEvents(connection, aggregateRootType, eventGroup, transaction);
                }
                else
                {
                    throw new ConcurrencyException(ConcurrencyException.DefaultConcurrencyExceptionMessage);
                }
            }
        }
        public virtual IEnumerable<SourcableEvent> GetEvents(string aggregateRootId, Type aggregateRootType, long minVersion, long maxVersion)
        {
            var evnts = new List<SourcableEvent>();
            var table = _sourcableEventTableProvider.GetTable(aggregateRootType);

            using (var connection = _connectionFactory.OpenConnection())
            {
                var eventType = _sourcableEventTypeProvider.GetSourcableEventType(aggregateRootType);
                var sql = string.Format("select * from [{0}] where AggregateRootId = @AggregateRootId and AggregateRootName = @AggregateRootName and Version >= @MinVersion and Version <= @MaxVersion order by Version asc", table);

                var eventList = EventQueryHelper.GetEventList(
                    eventType,
                    connection,
                    sql,
                    new
                    {
                        AggregateRootId = aggregateRootId,
                        AggregateRootName = GetAggregateRootName(aggregateRootType),
                        MinVersion = minVersion,
                        MaxVersion = maxVersion
                    });

                foreach (var evnt in eventList)
                {
                    var sourcableEvent = evnt as SourcableEvent;
                    sourcableEvent.AggregateRootType = aggregateRootType;
                    sourcableEvent.RawEvent = DeserializeEvent(GetEventType(sourcableEvent.Name), sourcableEvent.Data);
                    evnts.Add(sourcableEvent);
                }
            }

            return evnts;
        }

        #region Private Methods

        private void InsertSourcableEvents(IDbConnection connection, Type aggregateRootType, IEnumerable<SourcableEvent> evnts, IDbTransaction transaction)
        {
            var table = _sourcableEventTableProvider.GetTable(aggregateRootType);
            var aggregateRootName = GetAggregateRootName(aggregateRootType);

            foreach (var evnt in evnts)
            {
                evnt.AggregateRootName = aggregateRootName;
                evnt.Name = GetEventName(evnt.RawEvent.GetType());
                evnt.Data = SerializeEvent(evnt.RawEvent);
                connection.Insert(
                    new
                    {
                        UniqueId = evnt.UniqueId,
                        AggregateRootName = evnt.AggregateRootName,
                        AggregateRootId = evnt.AggregateRootId,
                        Name = evnt.Name,
                        Version = evnt.Version,
                        OccurredTime = evnt.OccurredTime,
                        Data = evnt.Data
                    }
                    , table, transaction);

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
            return _serializer.Serialize(evnt);
        }
        private object DeserializeEvent(Type eventType, string value)
        {
            return _serializer.Deserialize(value, eventType);
        }

        #endregion

        private class EventQueryHelper
        {
            private static MethodInfo _getEventListMethod = typeof(EventQueryHelper).GetMethod("GetEventList", BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
            private static EventQueryHelper _instance = new EventQueryHelper();

            public static IEnumerable GetEventList(Type sourcableEventType, IDbConnection connection, string sql, dynamic param)
            {
                return _getEventListMethod.MakeGenericMethod(sourcableEventType).Invoke(_instance, new object[] { connection, sql, param }) as IEnumerable;
            }

            private IEnumerable<TSourcableEvent> GetEventList<TSourcableEvent>(IDbConnection connection, string sql, object param) where TSourcableEvent : SourcableEvent
            {
                return connection.Query<TSourcableEvent>(sql, param);
            }
        }
    }
}
