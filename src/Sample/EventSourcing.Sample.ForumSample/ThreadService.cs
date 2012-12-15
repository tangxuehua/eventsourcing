using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.ForumSample
{
    public interface IThreadService
    {
        Thread Create(string subject, string body, Guid forumId, Guid authorId, int marks);
        void ChangeContent(Guid id, string subject, string body, int marks);
        void MarkAsRecommended(Guid id);
        void UnMarkAsRecommended(Guid id);
        void Close(Guid id);
        void MarkAsDeleted(Guid id);
        void Stick(Guid id);
        void CancelStick(Guid id);
    }

    public class ThreadService : IThreadService
    {
        private IContextManager _contextManager;

        public ThreadService(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public Thread Create(string subject, string body, Guid forumId, Guid authorId, int marks)
        {
            using (var context = _contextManager.GetContext())
            {
                var forum = context.Load<Forum>(forumId);
                var author = context.Load<User>(authorId);
                var thread = new Thread(subject, body, forum, author, marks);
                context.Add(thread);
                context.SaveChanges();
                return thread;
            }
        }

        public void ChangeContent(Guid id, string subject, string body, int marks)
        {
            using (var context = _contextManager.GetContext())
            {
                context.Load<Thread>(id).ChangeContent(subject, body, marks);
                context.SaveChanges();
            }
        }
        public void MarkAsRecommended(Guid id)
        {
            using (var context = _contextManager.GetContext())
            {
                context.Load<Thread>(id).MarkAsRecommended();
                context.SaveChanges();
            }
        }
        public void UnMarkAsRecommended(Guid id)
        {
            using (var context = _contextManager.GetContext())
            {
                context.Load<Thread>(id).UnMarkAsRecommended();
                context.SaveChanges();
            }
        }
        public void Close(Guid id)
        {
            using (var context = _contextManager.GetContext())
            {
                context.Load<Thread>(id).Close();
                context.SaveChanges();
            }
        }
        public void MarkAsDeleted(Guid id)
        {
            using (var context = _contextManager.GetContext())
            {
                context.Load<Thread>(id).MarkAsDeleted();
                context.SaveChanges();
            }
        }
        public void Stick(Guid id)
        {
            using (var context = _contextManager.GetContext())
            {
                context.Load<Thread>(id).Stick();
                context.SaveChanges();
            }
        }
        public void CancelStick(Guid id)
        {
            using (var context = _contextManager.GetContext())
            {
                context.Load<Thread>(id).CancelStick();
                context.SaveChanges();
            }
        }
    }
}
