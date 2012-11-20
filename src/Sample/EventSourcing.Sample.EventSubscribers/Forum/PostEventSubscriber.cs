using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.NHibernate;
using EventSourcing.Sample.Entities;
using EventSourcing.Sample.Model.Forum;

namespace EventSourcing.Sample.EventSubscribers
{
    public class PostEventSubscriber
    {
        private ISessionHelper _sessionHelper;

        public PostEventSubscriber(ISessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
        }

        [AsyncEventHandler]
        private void Handle(PostCreated evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                session.Save(ObjectHelper.CreateObject<PostEntity>(evnt));
            });
        }
    }
}
