using System.Linq;
using CodeSharp.EventSourcing;
using EventSourcing.Sample.Model.BookBorrowAndReturn;

namespace EventSourcing.Sample.EventSubscribers
{
    public class LibraryAccountEventSubscriber
    {
        private IDbConnectionFactory _connectionFactory;

        public LibraryAccountEventSubscriber(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [AsyncEventHandler]
        private void Handle(AccountCreated evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                conn.Insert(evnt, "EventSourcing_Sample_LibraryAccount");
            }
        }
        [AsyncEventHandler]
        private void Handle(BookBorrowed evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                var key = new { LibraryId = evnt.LibraryId, AccountId = evnt.AccountId, BookId = evnt.BookId };
                var borrowedBooks = conn.Query(key, "EventSourcing_Sample_BorrowedBook");

                if (borrowedBooks.Count() == 0)
                {
                    conn.Insert(evnt, "EventSourcing_Sample_BorrowedBook");
                }
                else
                {
                    var originalCount = (int)borrowedBooks.First().Count;
                    conn.Update(new { Count = originalCount + evnt.Count }, key, "EventSourcing_Sample_BorrowedBook");
                }
            }
        }
    }
}
