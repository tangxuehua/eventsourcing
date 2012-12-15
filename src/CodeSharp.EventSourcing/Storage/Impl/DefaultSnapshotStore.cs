//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Linq;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// Default empty implementation of snapshot store, not do any snapshot store.
    /// </summary>
    public class DefaultSnapshotStore : ISnapshotStore
    {
        #region Private Variables

        private ISerializer _serializer;
        private ITypeNameMappingProvider _typeNameMappingProvider;
        private ISnapshotTypeProvider _snapshotTypeProvider;
        private IDbConnectionFactory _connectionFactory;
        private ILogger _logger;

        #endregion

        #region Constructors

        public DefaultSnapshotStore(
            IDbConnectionFactory connectionFactory,
            ISerializer serializer,
            ITypeNameMappingProvider typeNameMappingProvider,
            ISnapshotTypeProvider snapshotTypeProvider,
            ILoggerFactory loggerFactory)
        {
            _serializer = serializer;
            _typeNameMappingProvider = typeNameMappingProvider;
            _snapshotTypeProvider = snapshotTypeProvider;
            _connectionFactory = connectionFactory;
            _logger = loggerFactory.Create("EventSourcing.DefaultSnapshotStore");
        }

        #endregion

        public virtual void StoreShapshot(Snapshot snapshot)
        {
            using (var connection = _connectionFactory.OpenConnection())
            {
                var table = Configuration.Instance.GetSetting<string>("snapshotTable");
                connection.Insert(
                    new
                    {
                        UniqueId = snapshot.UniqueId,
                        Name = GetSnapshotDataName(snapshot.Data.GetType()),
                        AggregateRootId = snapshot.AggregateRootId,
                        AggregateRootName = GetAggregateRootName(snapshot.AggregateRootType),
                        Version = snapshot.Version,
                        SerializedData = SerializeSnapshotData(snapshot.Data),
                        CreatedTime = snapshot.CreatedTime
                    }, table);
            }
        }
        public virtual Snapshot GetLastestSnapshot(string aggregateRootId, Type aggregateRootType)
        {
            using (var connection = _connectionFactory.OpenConnection())
            {
                var table = Configuration.Instance.GetSetting<string>("snapshotTable");
                var sql = "select * from {0} where AggregateRootId = @AggregateRootId and AggregateRootName = @AggregateRootName order by Version Desc";
                var snapshots = connection.Query<Snapshot>(
                    string.Format(sql, table),
                    new { AggregateRootId = aggregateRootId, AggregateRootName = GetAggregateRootName(aggregateRootType) });
                if (snapshots.Count() > 0)
                {
                    var snapshot = snapshots.First();
                    snapshot.AggregateRootType = aggregateRootType;
                    snapshot.Data = DeserializeSnapshotData(GetSnapshotDataType(snapshot.Name), snapshot.SerializedData);
                    return snapshot;
                }

                return null;
            }
        }

        #region Private Methods

        private string GetAggregateRootName(Type aggregateRootType)
        {
            return _typeNameMappingProvider.GetName(NameTypeMappingType.AggregateRootMapping, aggregateRootType);
        }
        private Type GetSnapshotDataType(string snapshotDataName)
        {
            return _typeNameMappingProvider.GetType(NameTypeMappingType.SnapshotMapping, snapshotDataName);
        }
        private string GetSnapshotDataName(Type snapshotDataType)
        {
            return _typeNameMappingProvider.GetName(NameTypeMappingType.SnapshotMapping, snapshotDataType);
        }
        private string SerializeSnapshotData(object snapshotData)
        {
            return _serializer.Serialize(snapshotData);
        }
        private object DeserializeSnapshotData(Type snapshotDataType, string snapshotSerializedData)
        {
            return _serializer.Deserialize(snapshotSerializedData, snapshotDataType);
        }

        #endregion
    }
}
