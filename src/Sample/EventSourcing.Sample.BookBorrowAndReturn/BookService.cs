using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.BookBorrowAndReturn
{
    public interface IBookService
    {
        /// <summary>
        /// 创建书本
        /// </summary>
        /// <param name="bookInfo"></param>
        /// <returns></returns>
        Book CreateBook(BookInfo bookInfo);
        /// <summary>
        /// 图书入库
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="count"></param>
        /// <param name="libraryId"></param>
        void AddBookToLibrary(Guid bookId, int count, Guid libraryId);
        /// <summary>
        /// 借书
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="accountId"></param>
        /// <param name="libraryId"></param>
        /// <param name="count"></param>
        void BorrowBook(Guid bookId, Guid accountId, Guid libraryId, int count);
        /// <summary>
        /// 还书
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="accountId"></param>
        /// <param name="libraryId"></param>
        /// <param name="count"></param>
        void ReturnBook(Guid bookId, Guid accountId, Guid libraryId, int count);
    }
    public class BookService : IBookService
    {
        private IContextManager _contextManager;

        public BookService(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public Book CreateBook(BookInfo bookInfo)
        {
            using (var context = _contextManager.GetContext())
            {
                var book = new Book(bookInfo);
                context.Add(book);
                context.SaveChanges();
                return book;
            }
        }
        public void AddBookToLibrary(Guid bookId, int count, Guid libraryId)
        {
            using (var context = _contextManager.GetContext())
            {
                var book = context.Load<Book>(bookId);
                var libarary = context.Load<Library>(libraryId);
                libarary.StoreBook(book, count);
                context.SaveChanges();
            }
        }
        public void BorrowBook(Guid bookId, Guid accountId, Guid libraryId, int count)
        {
            using (var context = _contextManager.GetContext())
            {
                var book = context.Load<Book>(bookId);
                var library = context.Load<Library>(libraryId);
                var account = context.Load<LibraryAccount>(accountId);
                new BorrowBookContext(account.ActAs<IBorrower>(), library).Interaction(book, count);
                context.SaveChanges();
            }
        }
        public void ReturnBook(Guid bookId, Guid accountId, Guid libraryId, int count)
        {
            using (var context = _contextManager.GetContext())
            {
                var book = context.Load<Book>(bookId);
                var library = context.Load<Library>(libraryId);
                var account = context.Load<LibraryAccount>(accountId);
                new ReturnBookContext(account.ActAs<IBorrower>(), library).Interaction(book, count);
                context.SaveChanges();
            }
        }
    }

    //DCI (data,role,context,interaction) example
    public class BorrowBookContext
    {
        private IBorrower _borrower;
        private Library _library;

        public BorrowBookContext(IBorrower borrower, Library library)
        {
            _borrower = borrower;
            _library = library;
        }

        public void Interaction(Book book, int count)
        {
            _borrower.BorrowBook(_library, book, count);
        }
    }
    public class ReturnBookContext
    {
        private IBorrower _borrower;
        private Library _library;

        public ReturnBookContext(IBorrower borrower, Library library)
        {
            _borrower = borrower;
            _library = library;
        }

        public void Interaction(Book book, int count)
        {
            _borrower.ReturnBook(_library, book, count);
        }
    }
}
