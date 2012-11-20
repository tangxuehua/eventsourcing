//Copyright (c) CodeSharp.  All rights reserved.

namespace CodeSharp.EventSourcing
{
    public class TopContext : ContextBase
    {
        private IContextLifetimeManager _contextLifetimeManager;

        public override bool IsChildContext
        {
            get { return false; }
        }
        public TopContext() : base()
        {
            _contextLifetimeManager = ObjectContainer.Resolve<IContextLifetimeManager>();
        }
        public override void Dispose()
        {
            _contextLifetimeManager.Remove(this);
            _logger.Debug("Top context removed and disposed.");
        }
    }
}
