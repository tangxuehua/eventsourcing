using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.NHibernate;
using EventSourcing.Sample.Entities;
using EventSourcing.Sample.Model.Forum;

namespace EventSourcing.Sample.EventSubscribers
{
    public class ForumEventSubscriber
    {
        private ISessionHelper _sessionHelper;

        public ForumEventSubscriber(ISessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
        }

        [AsyncEventHandler]
        private void Handle(ForumCreated evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                var forum = new ForumEntity
                {
                    Id = evnt.Id,
                    Name = evnt.Name,
                    TotalThread = evnt.State.TotalThread,
                    TotalPost = evnt.State.TotalPost,
                    LatestThreadId = evnt.State.LatestThreadId,
                    LatestPostAuthorId = evnt.State.LatestPostAuthorId
                };
                session.Save(forum);
            });
        }
        [AsyncEventHandler]
        private void Handle(ForumStateChanged evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                var forum = session.Get<ForumEntity>(evnt.Id);
                forum.TotalThread = evnt.State.TotalThread;
                forum.TotalPost = evnt.State.TotalPost;
                forum.LatestThreadId = evnt.State.LatestThreadId;
                forum.LatestPostAuthorId = evnt.State.LatestPostAuthorId;
                session.Update(forum);
            });
        }
    }
}
