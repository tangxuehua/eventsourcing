using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.NHibernate;
using EventSourcing.Sample.Entities;
using EventSourcing.Sample.Model.BookBorrowAndReturn;

namespace EventSourcing.Sample.EventSubscribers
{
    public class BookEventSubscriber
    {
        private ISessionHelper _sessionHelper;

        public BookEventSubscriber(ISessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
        }

        [AsyncEventHandler]
        private void Handle(BookCreated evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                var book = new BookEntity
                {
                    Id = evnt.Id,
                    Name = evnt.BookInfo.Name,
                    Description = evnt.BookInfo.Description,
                    Author = evnt.BookInfo.Author,
                    ISBN = evnt.BookInfo.ISBN,
                    Publisher = evnt.BookInfo.Publisher
                };
                session.Save(book);
            });
        }
    }
}
