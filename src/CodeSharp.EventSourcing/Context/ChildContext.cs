//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    public class ChildContext : ContextBase
    {
        private IContext _parent;

        public ChildContext(IContext parent) : base()
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            _parent = parent;
        }

        public override bool IsChildContext
        {
            get { return true; }
        }
        public override void Add(AggregateRoot instance)
        {
            _parent.Add(instance);
        }
        public override T Load<T>(object id)
        {
            return _parent.Load<T>(id);
        }
        public override void SaveChanges() { }
        public override void Dispose() { }
    }
}
