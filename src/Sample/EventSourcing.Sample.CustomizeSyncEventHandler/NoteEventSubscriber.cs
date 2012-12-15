using System.Data;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.CustomizeSyncEventHandler
{
    public class NoteEventSubscriber : ISyncEventSubscriber<NoteCreated>, ISyncEventSubscriber<NoteTitleChanged>
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public NoteEventSubscriber(ICurrentDbTransactionProvider transactionProvider)
        {
            _transaction = transactionProvider.CurrentTransaction;
            _connection = _transaction.Connection;
        }

        public void Handle(NoteCreated evnt)
        {
            _connection.Insert(evnt, "EventSourcing_Sample_Note", _transaction);
        }
        public void Handle(NoteTitleChanged evnt)
        {
            _connection.Update(
                new { Title = evnt.Title, UpdatedTime = evnt.UpdatedTime },
                new { Id = evnt.Id },
                "EventSourcing_Sample_Note",
                _transaction);
        }
    }
}
