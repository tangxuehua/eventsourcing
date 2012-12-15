using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.ForumSample
{
    public interface IForumService
    {
        Forum Create(string name);
    }
    public class ForumService : IForumService
    {
        private IContextManager _contextManager;

        public ForumService(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public Forum Create(string name)
        {
            using (var context = _contextManager.GetContext())
            {
                var forum = new Forum(name);
                context.Add(forum);
                context.SaveChanges();
                return forum;
            }
        }
    }
}
