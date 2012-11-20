using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.NHibernate;
using EventSourcing.Sample.Entities;
using EventSourcing.Sample.Model.BookBorrowAndReturn;

namespace EventSourcing.Sample.EventSubscribers
{
    public class HandlingEventEventSubscriber
    {
        private ISessionHelper _sessionHelper;

        public HandlingEventEventSubscriber(ISessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
        }

        [AsyncEventHandler]
        private void Handle(HandlingEventCreated evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                session.Save(ObjectHelper.CreateObject<HandlingEventEntity>(evnt));
            });
        }
    }
}
