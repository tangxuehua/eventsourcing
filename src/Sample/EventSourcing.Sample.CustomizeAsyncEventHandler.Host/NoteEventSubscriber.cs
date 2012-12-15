using System.Data;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.CustomizeAsyncEventHandler.Host
{
    public class NoteEventSubscriber : IAsyncEventSubscriber<NoteCreated>, IAsyncEventSubscriber<NoteTitleChanged>
    {
        private IDbConnectionFactory _connectionFactory;

        public NoteEventSubscriber(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Handle(NoteCreated evnt)
        {
            using (var connection = _connectionFactory.OpenConnection())
            {
                connection.Insert(evnt, "EventSourcing_Sample_Note");
            }
        }
        public void Handle(NoteTitleChanged evnt)
        {
            using (var connection = _connectionFactory.OpenConnection())
            {
                connection.Update(
                new { Title = evnt.Title, UpdatedTime = evnt.UpdatedTime },
                new { Id = evnt.Id },
                "EventSourcing_Sample_Note");
            }
        }
    }
}
