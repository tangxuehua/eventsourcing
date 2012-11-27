using CodeSharp.EventSourcing;
using EventSourcing.Sample.Model.BookBorrowAndReturn;

namespace EventSourcing.Sample.EventSubscribers
{
    public class LibraryEventSubscriber
    {
        private IDbConnectionFactory _connectionFactory;

        public LibraryEventSubscriber(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [AsyncEventHandler]
        private void Handle(LibraryCreated evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                conn.Insert(evnt, "EventSourcing_Sample_Library");
            }
        }
        [AsyncEventHandler]
        private void Handle(NewBookStored evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                conn.Insert(evnt, "EventSourcing_Sample_BookStoreItem");
            }
        }
        [AsyncEventHandler]
        private void Handle(BookCountUpdated evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                conn.Update(
                    new { Count = evnt.Count },
                    new { LibraryId = evnt.LibraryId, BookId = evnt.BookId },
                    "EventSourcing_Sample_BookStoreItem");
            }
        }
        [AsyncEventHandler]
        private void Handle(BookLent evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                var key = new { LibraryId = evnt.LibraryId, BookId = evnt.BookId };
                var count = conn.GetValue<int>(key, "EventSourcing_Sample_BookStoreItem", "Count");
                conn.Update(
                    new { Count = count - evnt.Count },
                    key,
                    "EventSourcing_Sample_BookStoreItem");
            }
        }
        [AsyncEventHandler]
        private void Handle(BookReceived evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                var key = new { LibraryId = evnt.LibraryId, BookId = evnt.BookId };
                var count = conn.GetValue<int>(key, "EventSourcing_Sample_BookStoreItem", "Count");
                conn.Update(
                    new { Count = count + evnt.Count },
                    key,
                    "EventSourcing_Sample_BookStoreItem");
            }
        }
    }
}
