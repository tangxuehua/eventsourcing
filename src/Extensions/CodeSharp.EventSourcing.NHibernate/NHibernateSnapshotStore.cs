//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;

namespace CodeSharp.EventSourcing.NHibernate
{
    /// <summary>
    /// NHibernate implementation of snapshot store.
    /// </summary>
    public class NHibernateSnapshotStore : ISnapshotStore
    {
        #region Private Variables

        private ISerializer _serializer;
        private ITypeNameMappingProvider _typeNameMappingProvider;
        private ISnapshotTypeProvider _snapshotTypeProvider;
        private ISessionFactory _sessionFactory;
        private ILogger _logger;

        #endregion

        #region Constructors

        public NHibernateSnapshotStore(
            ISessionFactory sessionFactory,
            ISerializer serializer,
            ITypeNameMappingProvider typeNameMappingProvider,
            ISnapshotTypeProvider snapshotTypeProvider,
            ILoggerFactory loggerFactory)
        {
            _serializer = serializer;
            _typeNameMappingProvider = typeNameMappingProvider;
            _snapshotTypeProvider = snapshotTypeProvider;
            _sessionFactory = sessionFactory;
            _logger = loggerFactory.Create("EventSourcing.NHibernateSnapshotStore");
        }

        #endregion

        public virtual void StoreShapshot(Snapshot snapshot)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        snapshot.AggregateRootName = GetAggregateRootName(snapshot.AggregateRootType);
                        snapshot.Name = GetSnapshotDataName(snapshot.Data.GetType());
                        snapshot.SerializedData = SerializeSnapshotData(snapshot.Data);
                        session.SaveOrUpdate(snapshot);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.Error("Unknown error when storing snapshot.", ex);
                        throw;
                    }
                }
            }
        }
        public virtual Snapshot GetLastestSnapshot(string aggregateRootId, Type aggregateRootType)
        {
            var session = _sessionFactory.OpenSession();
            var criteria = session.CreateCriteria<Snapshot>();

            criteria.Add(Restrictions.Eq("aggregateRootId", aggregateRootId));
            criteria.Add(Restrictions.Eq("AggregateRootName", GetAggregateRootName(aggregateRootType)));
            criteria.AddOrder(new Order("Version", false));

            criteria = criteria.SetFirstResult(0).SetMaxResults(1);

            var snapshots = criteria.List<Snapshot>();

            if (snapshots.Count > 0)
            {
                var snapshot = snapshots.First();
                snapshot.AggregateRootType = aggregateRootType;
                snapshot.Data = DeserializeSnapshotData(GetSnapshotDataType(snapshot.Name), snapshot.SerializedData);
                return snapshot;
            }

            return null;
        }

        #region Private Methods

        private Type GetAggregateRootType(string aggregateRootName)
        {
            return _typeNameMappingProvider.GetType(NameTypeMappingType.AggregateRootMapping, aggregateRootName);
        }
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
