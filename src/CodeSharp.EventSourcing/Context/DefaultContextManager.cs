//Copyright (c) CodeSharp.  All rights reserved.

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 默认的Context管理器
    /// </summary>
    public class DefaultContextManager : IContextManager
    {
        private readonly string DefaultContextKey = "ContextManager.DefaultContextKey";
        private IContextLifetimeManager _contextLifetimeManager;
        private ILogger _logger;

        public DefaultContextManager(IContextLifetimeManager contextLifetimeManager, ILoggerFactory loggerFactory)
        {
            _contextLifetimeManager = contextLifetimeManager;
            _logger = loggerFactory.Create("EventSourcing.DefaultContextManager");
        }

        public virtual IContext GetContext()
        {
            var context = _contextLifetimeManager.Find(DefaultContextKey) as IContext;

            if (context == null)
            {
                context = new TopContext();
                _contextLifetimeManager.Store(DefaultContextKey, context);
                _logger.Debug("Top context created and stored.");
            }
            else
            {
                context = new ChildContext(context);
                _logger.Debug("Child context created.");
            }

            return context;
        }
    }
}
