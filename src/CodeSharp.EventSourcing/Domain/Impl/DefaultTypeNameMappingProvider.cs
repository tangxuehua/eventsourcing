//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 默认的类型与名称映射信息提供者
    /// </summary>
    public class DefaultTypeNameMappingProvider : ITypeNameMappingProvider
    {
        #region Private Variables

        private IEventTypeProvider _eventTypeProvider;
        private ISnapshotTypeProvider _snapshotTypeProvider;
        private IDictionary<NameTypeMappingType, IDictionary<string, Type>> _mappings;

        #endregion

        #region Constructors

        public DefaultTypeNameMappingProvider(IEventTypeProvider eventTypeProvider, ISnapshotTypeProvider snapshotTypeProvider)
        {
            _eventTypeProvider = eventTypeProvider;
            _snapshotTypeProvider = snapshotTypeProvider;
            _mappings = new Dictionary<NameTypeMappingType, IDictionary<string, Type>>();
        }

        #endregion

        #region Public Methods

        public Type GetType(NameTypeMappingType mappingType, string name)
        {
            var nameTypes = GetMappingDictionary(mappingType);

            if (!nameTypes.ContainsKey(name))
            {
                var message = string.Format("无法为指定的名称'{0}'找到对应的类型", name);
                throw new ArgumentOutOfRangeException("name", name, message);
            }
            return nameTypes[name];
        }
        public string GetName(NameTypeMappingType mappingType, Type type)
        {
            var nameTypes = GetMappingDictionary(mappingType);

            if (!nameTypes.Values.Any(x => x == type))
            {
                var message = string.Format("无法为指定的类型'{0}'找到对应的名称", type);
                throw new ArgumentOutOfRangeException("type", type, message);
            }

            return nameTypes.Single(x => x.Value == type).Key;
        }
        public bool IsTypeExist(NameTypeMappingType mappingType, Type type)
        {
            return GetMappingDictionary(mappingType).Values.Any(x => x == type);
        }
        public bool IsNameExist(NameTypeMappingType mappingType, string name)
        {
            return GetMappingDictionary(mappingType).ContainsKey(name);
        }
        public void RegisterMappings(NameTypeMappingType mappingType, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                if (mappingType == NameTypeMappingType.AggregateRootMapping)
                {
                    foreach (var type in assembly.GetTypes().Where(x => TypeUtils.IsAggregateRoot(x)))
                    {
                        if (!IsTypeExist(mappingType, type))
                        {
                            RegisterMapping(mappingType, type.FullName, type);
                        }
                    }
                }
                else if (mappingType == NameTypeMappingType.SourcableEventMapping)
                {
                    foreach (var type in assembly.GetTypes().Where(x => _eventTypeProvider.IsSourcableEvent(x)))
                    {
                        if (!IsTypeExist(mappingType, type))
                        {
                            RegisterMapping(mappingType, type.FullName, type);
                        }
                    }
                }
                else if (mappingType == NameTypeMappingType.SnapshotMapping)
                {
                    foreach (var type in assembly.GetTypes().Where(x => _snapshotTypeProvider.IsSnapshot(x)))
                    {
                        if (!IsTypeExist(mappingType, type))
                        {
                            RegisterMapping(mappingType, type.FullName, type);
                        }
                    }
                }
            }
        }
        public void RegisterMapping(NameTypeMappingType mappingType, string name, Type type)
        {
            var nameTypes = GetMappingDictionary(mappingType);

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            //以下验证确保类型与名称之间是一一对应关系
            if (nameTypes.ContainsKey(name))
            {
                Type otherType = nameTypes[name];
                throw new EventSourcingException(string.Format("不能为类型{0}注册名称{1}，因为已经有另外一个类型{2}使用了该名称", type.FullName, name, otherType.FullName));
            }
            if (nameTypes.Values.Any(x => x == type))
            {
                string otherName = nameTypes.Single(x => x.Value == type).Key;
                throw new EventSourcingException(string.Format("不能为名称{0}注册类型{1}，因为已经有另外一个名称{2}使用了该类型", name, type.FullName, otherName));
            }

            nameTypes.Add(name, type);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 根据映射种类获取一个存放了类型与其名称之间映射关系的字典
        /// </summary>
        private IDictionary<string, Type> GetMappingDictionary(NameTypeMappingType mappingType)
        {
            if (!_mappings.ContainsKey(mappingType))
            {
                _mappings.Add(mappingType, new Dictionary<string, Type>());
            }
            return _mappings[mappingType];
        }

        #endregion
    }
}
