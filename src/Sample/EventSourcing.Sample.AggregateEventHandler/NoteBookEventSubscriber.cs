using System.Data;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.AggregateEventHandler
{
    public class NoteBookEventSubscriber
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public NoteBookEventSubscriber(ICurrentDbTransactionProvider transactionProvider)
        {
            _transaction = transactionProvider.CurrentTransaction;
            _connection = _transaction.Connection;
        }

        [SyncEventHandler]
        public void Handle(NoteBookCreated evnt)
        {
            _connection.Insert(evnt, "EventSourcing_Sample_NoteBook", _transaction);
        }

        [SyncEventHandler]
        public void Handle(TotalNoteCountChanged evnt)
        {
            _connection.Update(
                new { TotalNoteCount = evnt.TotalNoteCount },
                new { Id = evnt.BookId },
                "EventSourcing_Sample_NoteBook",
                _transaction);
        }
    }
}
