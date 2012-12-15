using System.Data;
using System.Linq;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.BookBorrowAndReturn
{
    public class LibraryAccountEventSubscriber
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public LibraryAccountEventSubscriber(ICurrentDbTransactionProvider transactionProvider)
        {
            _transaction = transactionProvider.CurrentTransaction;
            _connection = _transaction.Connection;
        }

        [SyncEventHandler]
        private void Handle(AccountCreated evnt)
        {
            _connection.Insert(evnt, "EventSourcing_Sample_LibraryAccount", _transaction);
        }
        [SyncEventHandler]
        private void Handle(BookBorrowed evnt)
        {
            var key = new { LibraryId = evnt.LibraryId, AccountId = evnt.AccountId, BookId = evnt.BookId };
            var borrowedBooks = _connection.Query(key, "EventSourcing_Sample_BorrowedBook", _transaction);

            if (borrowedBooks.Count() == 0)
            {
                _connection.Insert(evnt, "EventSourcing_Sample_BorrowedBook", _transaction);
            }
            else
            {
                var originalCount = (int)borrowedBooks.First().Count;
                _connection.Update(new { Count = originalCount + evnt.Count }, key, "EventSourcing_Sample_BorrowedBook", _transaction);
            }
        }
    }
}
