using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.NHibernate;
using EventSourcing.Sample.Entities;
using EventSourcing.Sample.Model.BookBorrowAndReturn;

namespace EventSourcing.Sample.EventSubscribers
{
    public class LibraryAccountEventSubscriber
    {
        private ISessionHelper _sessionHelper;

        public LibraryAccountEventSubscriber(ISessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
        }

        [AsyncEventHandler]
        private void Handle(AccountCreated evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                session.Save(ObjectHelper.CreateObject<LibraryAccountEntity>(evnt));
            });
        }
        [AsyncEventHandler]
        private void Handle(BookBorrowed evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                var borrowedBook = _sessionHelper.QueryUnique<BorrowedBookEntity>(session, new { AccountId = evnt.AccountId, BookId = evnt.BookId });
                if (borrowedBook == null)
                {
                    session.Save(ObjectHelper.CreateObject<BorrowedBookEntity>(evnt));
                }
                else
                {
                    borrowedBook.Count += evnt.Count;
                    session.Update(borrowedBook);
                }
            });
        }
    }
}
