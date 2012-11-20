using CodeSharp.EventSourcing;
using ForumModel = EventSourcing.Sample.Model.Forum.Forum;

namespace EventSourcing.Sample.Application
{
    public interface IForumService
    {
        ForumModel Create(string name);
    }
    public class ForumService : IForumService
    {
        private IContextManager _contextManager;

        public ForumService(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public ForumModel Create(string name)
        {
            using (var context = _contextManager.GetContext())
            {
                var forum = new ForumModel(name);
                context.Add(forum);
                context.SaveChanges();
                return forum;
            }
        }
    }
}
