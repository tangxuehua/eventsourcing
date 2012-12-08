using System.Data;
using CodeSharp.EventSourcing;
using EventSourcing.Sample.Model.BookBorrowAndReturn;

namespace EventSourcing.Sample.EventSubscribers
{
    public class BookEventSubscriber
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public BookEventSubscriber(ICurrentDbTransactionProvider transactionProvider)
        {
            _transaction = transactionProvider.CurrentTransaction;
            _connection = _transaction.Connection;
        }

        [SyncEventHandler]
        private void Handle(BookCreated evnt)
        {
            _connection.Insert(
                new
                {
                    Id = evnt.Id,
                    Name = evnt.BookInfo.Name,
                    Description = evnt.BookInfo.Description,
                    Author = evnt.BookInfo.Author,
                    ISBN = evnt.BookInfo.ISBN,
                    Publisher = evnt.BookInfo.Publisher
                }, "EventSourcing_Sample_Book", _transaction);
        }
    }
}
