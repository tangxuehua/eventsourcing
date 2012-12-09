using CodeSharp.EventSourcing;
using NHibernate;
using NHibernate.Criterion;

namespace EventSourcing.Sample.NHibernateIntegration
{
    public class NoteEventSubscriber
    {
        private ISession _session;

        public NoteEventSubscriber(ICurrentSessionProvider sessionProvider)
        {
            _session = sessionProvider.CurrentSession;
        }

        [SyncEventHandler]
        public void Handle(NoteCreated evnt)
        {
            var note = new NoteEntity
            {
                Id = evnt.Id,
                Title = evnt.Title,
                CreatedTime = evnt.CreatedTime,
                UpdatedTime = evnt.UpdatedTime
            };
            _session.Save(note);
        }

        [SyncEventHandler]
        public void Handle(NoteTitleChanged evnt)
        {
            var note = _session.CreateCriteria<NoteEntity>().Add(Expression.Eq("Id", evnt.Id)).UniqueResult<NoteEntity>();
            note.Title = evnt.Title;
            note.UpdatedTime = evnt.UpdatedTime;
            _session.Update(note);
        }
    }
}
