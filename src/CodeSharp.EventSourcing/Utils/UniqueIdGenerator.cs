//Copyright (c) CodeSharp.  All rights reserved.

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 唯一标识生成器接口定义
    /// </summary>
    public interface IUniqueIdGenerator<TUniqueId>
    {
        /// <summary>
        /// 生成一个唯一标识
        /// </summary>
        TUniqueId GenerateUniqueId<TEntity>(params object[] parameters);
    }
    /// <summary>
    /// 唯一标识生成器
    /// </summary>
    public static class UniqueIdGenerator
    {
        public static TUniqueId GenerateUniqueId<TEntity, TUniqueId>(params object[] parameters) where TUniqueId : UniqueId
        {
            return ObjectContainer.Resolve<IUniqueIdGenerator<TUniqueId>>().GenerateUniqueId<TEntity>(parameters);
        }
    }
    public abstract class UniqueId
    {
        public string EntityName { get; protected set; }
        public string Value { get; protected set; }

        protected UniqueId() { }
        public UniqueId(string entityName) { EntityName = entityName; }
    }
    public abstract class UniqueId<TValue> : UniqueId
    {
        public new TValue Value
        {
            get
            {
                if (base.Value != null)
                {
                    return EventSourcing.Utils.ConvertType<TValue>(base.Value);
                }
                return default(TValue);
            }
            set
            {
                base.Value = EventSourcing.Utils.ConvertType<string>(value);
            }
        }

        protected UniqueId() { }
        public UniqueId(string id) : base(id) { }

        public abstract TValue UpdateValue();
    }
    public class IntUniqueId : UniqueId<int>
    {
        public override int UpdateValue()
        {
            return base.Value++;
        }
    }
    public class LongUniqueId : UniqueId<long>
    {
        public override long UpdateValue()
        {
            return base.Value++;
        }
    }
}
