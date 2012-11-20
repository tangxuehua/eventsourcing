using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.NHibernate;
using EventSourcing.Sample.Entities;
using EventSourcing.Sample.Model.Forum;

namespace EventSourcing.Sample.EventSubscribers
{
    public class UserEventSubscriber
    {
        private ISessionHelper _sessionHelper;

        public UserEventSubscriber(ISessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
        }

        [AsyncEventHandler]
        private void Handle(UserCreated evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                session.Save(ObjectHelper.CreateObject<UserEntity>(evnt));
            });
        }
    }
}
