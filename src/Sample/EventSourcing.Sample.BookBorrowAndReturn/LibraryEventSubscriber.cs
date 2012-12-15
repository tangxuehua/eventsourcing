using System.Data;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.BookBorrowAndReturn
{
    public class LibraryEventSubscriber
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public LibraryEventSubscriber(ICurrentDbTransactionProvider transactionProvider)
        {
            _transaction = transactionProvider.CurrentTransaction;
            _connection = _transaction.Connection;
        }

        [SyncEventHandler]
        private void Handle(LibraryCreated evnt)
        {
            _connection.Insert(evnt, "EventSourcing_Sample_Library", _transaction);
        }
        [SyncEventHandler]
        private void Handle(NewBookStored evnt)
        {
            _connection.Insert(evnt, "EventSourcing_Sample_BookStoreItem", _transaction);
        }
        [SyncEventHandler]
        private void Handle(BookCountUpdated evnt)
        {
            _connection.Update(
                    new { Count = evnt.Count },
                    new { LibraryId = evnt.LibraryId, BookId = evnt.BookId },
                    "EventSourcing_Sample_BookStoreItem", _transaction);
        }
        [SyncEventHandler]
        private void Handle(BookLent evnt)
        {
            var key = new { LibraryId = evnt.LibraryId, BookId = evnt.BookId };
            var count = _connection.GetValue<int>(key, "EventSourcing_Sample_BookStoreItem", "Count", _transaction);
            _connection.Update(
                new { Count = count - evnt.Count },
                key,
                "EventSourcing_Sample_BookStoreItem", _transaction);
        }
        [SyncEventHandler]
        private void Handle(BookReceived evnt)
        {
            var key = new { LibraryId = evnt.LibraryId, BookId = evnt.BookId };
            var count = _connection.GetValue<int>(key, "EventSourcing_Sample_BookStoreItem", "Count", _transaction);
            _connection.Update(
                new { Count = count + evnt.Count },
                key,
                "EventSourcing_Sample_BookStoreItem", _transaction);
        }
    }
}
