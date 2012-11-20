using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.NHibernate;
using EventSourcing.Sample.Entities;
using EventSourcing.Sample.Model.Forum;

namespace EventSourcing.Sample.EventSubscribers
{
    public class ThreadEventSubscriber
    {
        private ISessionHelper _sessionHelper;

        public ThreadEventSubscriber(ISessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
        }

        [AsyncEventHandler]
        private void Handle(ThreadCreated evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                var thread = ObjectHelper.CreateObject<ThreadEntity>(evnt);
                thread.IsStick = evnt.StickInfo.IsStick;
                thread.StickDate = evnt.StickInfo.StickDate;
                session.Save(thread);
            });
        }
        [AsyncEventHandler]
        private void Handle(ContentChanged evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                var thread = session.Get<ThreadEntity>(evnt.Id);
                ObjectHelper.UpdateObject<ThreadEntity, ContentChanged>(thread, evnt,
                    x => x.Subject,
                    x => x.Body,
                    x => x.Marks);
                session.Update(thread);
            });
        }
        [AsyncEventHandler]
        private void Handle(ThreadStatusChanged evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                var thread = session.Get<ThreadEntity>(evnt.Id);
                thread.Status = (int)evnt.Status;
                session.Update(thread);
            });
        }
        [AsyncEventHandler]
        private void Handle(ThreadStickInfoChanged evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                var thread = session.Get<ThreadEntity>(evnt.Id);
                thread.IsStick = evnt.StickInfo.IsStick;
                thread.StickDate = evnt.StickInfo.StickDate;
                session.Update(thread);
            });
        }
    }
}
