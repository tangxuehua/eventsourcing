using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.ForumSample
{
    public interface IPostService
    {
        Post Create(string body, Guid threadId, Guid authorId);
    }
    public class PostService : IPostService
    {
        private IContextManager _contextManager;

        public PostService(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public Post Create(string body, Guid threadId, Guid authorId)
        {
            using (var context = _contextManager.GetContext())
            {
                var thread = context.Load<Thread>(threadId);
                var author = context.Load<User>(authorId);
                var post = new Post(body, thread, author);
                context.Add(post);
                context.SaveChanges();
                return post;
            }
        }
    }
}
