using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.NHibernate;
using EventSourcing.Sample.Entities;
using EventSourcing.Sample.Model.BookBorrowAndReturn;

namespace EventSourcing.Sample.EventSubscribers
{
    public class LibraryEventSubscriber
    {
        private ISessionHelper _sessionHelper;

        public LibraryEventSubscriber(ISessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
        }

        [AsyncEventHandler]
        private void Handle(LibraryCreated evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                session.Save(ObjectHelper.CreateObject<LibraryEntity>(evnt));
            });
        }
        [AsyncEventHandler]
        private void Handle(NewBookStored evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                session.Save(ObjectHelper.CreateObject<BookStoreItemEntity>(evnt));
            });
        }
        [AsyncEventHandler]
        private void Handle(BookCountUpdated evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                var storeItem = _sessionHelper.QueryUnique<BookStoreItemEntity>(session, new { LibraryId = evnt.LibraryId, BookId = evnt.BookId });
                storeItem.Count = evnt.Count;
                session.Update(storeItem);
            });
        }
        [AsyncEventHandler]
        private void Handle(BookLent evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                var storeItem = _sessionHelper.QueryUnique<BookStoreItemEntity>(session, new { LibraryId = evnt.LibraryId, BookId = evnt.BookId });
                storeItem.Count -= evnt.Count;
                session.Update(storeItem);
            });
        }
        [AsyncEventHandler]
        private void Handle(BookReceived evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                var storeItem = _sessionHelper.QueryUnique<BookStoreItemEntity>(session, new { LibraryId = evnt.LibraryId, BookId = evnt.BookId });
                storeItem.Count += evnt.Count;
                session.Update(storeItem);
            });
        }
    }
}
