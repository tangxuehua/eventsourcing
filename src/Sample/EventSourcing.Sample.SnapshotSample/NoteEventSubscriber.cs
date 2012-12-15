using System.Data;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.SnapshotSample
{
    public class NoteEventSubscriber
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public NoteEventSubscriber(ICurrentDbTransactionProvider transactionProvider)
        {
            _transaction = transactionProvider.CurrentTransaction;
            _connection = _transaction.Connection;
        }

        [SyncEventHandler]
        public void Handle(NoteCreated evnt)
        {
            _connection.Insert(evnt, "EventSourcing_Sample_Note", _transaction);
        }

        [SyncEventHandler]
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
