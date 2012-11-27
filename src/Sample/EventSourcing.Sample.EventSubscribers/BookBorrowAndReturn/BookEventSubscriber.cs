using CodeSharp.EventSourcing;
using EventSourcing.Sample.Model.BookBorrowAndReturn;

namespace EventSourcing.Sample.EventSubscribers
{
    public class BookEventSubscriber
    {
        private IDbConnectionFactory _connectionFactory;

        public BookEventSubscriber(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [AsyncEventHandler]
        private void Handle(BookCreated evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                conn.Insert(
                    new
                    {
                        Id = evnt.Id,
                        Name = evnt.BookInfo.Name,
                        Description = evnt.BookInfo.Description,
                        Author = evnt.BookInfo.Author,
                        ISBN = evnt.BookInfo.ISBN,
                        Publisher = evnt.BookInfo.Publisher
                    }, "EventSourcing_Sample_Book");
            }
        }
    }
}
