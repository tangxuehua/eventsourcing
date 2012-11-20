using System.Linq;
using CodeSharp.EventSourcing;
using EventSourcing.Sample.Application;
using EventSourcing.Sample.Model.BookBorrowAndReturn;
using NUnit.Framework;

namespace EventSourcing.Sample.Test
{
    [TestFixture]
    public class BookBorrowAndReturnTest : TestBase
    {
        [Test]
        public void Test()
        {
            var accountService = ObjectContainer.Resolve<ILibraryAccountService>();
            var libraryService = ObjectContainer.Resolve<ILibraryService>();
            var bookService = ObjectContainer.Resolve<IBookService>();
            var contextManager = ObjectContainer.Resolve<IContextManager>();

            //创建借书账号
            var account = accountService.Create(RandomString(), RandomString());

            //创建三本书
            var book1Info = new BookInfo("Java Programming", "ISBN-001", "John", "Publisher1", string.Empty);
            var book2Info = new BookInfo(".Net Programming", "ISBN-002", "Jim", "Publisher2", string.Empty);
            var book3Info = new BookInfo("Mono Develop", "ISBN-003", "Richer", "Publisher3", string.Empty);
            var book1 = bookService.CreateBook(book1Info);
            var book2 = bookService.CreateBook(book2Info);
            var book3 = bookService.CreateBook(book3Info);

            //创建图书馆
            var library = libraryService.Create(RandomString());

            //图书入库
            bookService.AddBookToLibrary(book1.Id, 5, library.Id);
            bookService.AddBookToLibrary(book2.Id, 5, library.Id);
            bookService.AddBookToLibrary(book3.Id, 5, library.Id);

            //借书
            bookService.BorrowBook(book1.Id, account.Id, library.Id, 2);
            bookService.BorrowBook(book1.Id, account.Id, library.Id, 1);
            bookService.BorrowBook(book2.Id, account.Id, library.Id, 5);
            bookService.BorrowBook(book3.Id, account.Id, library.Id, 2);

            //还书
            bookService.ReturnBook(book1.Id, account.Id, library.Id, 1);
            bookService.ReturnBook(book1.Id, account.Id, library.Id, 1);
            bookService.ReturnBook(book2.Id, account.Id, library.Id, 5);
            bookService.ReturnBook(book3.Id, account.Id, library.Id, 1);

            //Assert图书馆剩余书本是否正确
            using (var context = contextManager.GetContext())
            {
                library = context.Load<Library>(library.Id);
            }
            Assert.AreEqual(4, library.BookStoreItems.Single(x => x.BookId == book1.Id).Count);
            Assert.AreEqual(5, library.BookStoreItems.Single(x => x.BookId == book2.Id).Count);
            Assert.AreEqual(4, library.BookStoreItems.Single(x => x.BookId == book3.Id).Count);

            //Assert账号借到的书本是否正确
            using (var context = contextManager.GetContext())
            {
                account = context.Load<LibraryAccount>(account.Id);
            }
            Assert.AreEqual(2, account.ActAs<IBorrower>().BorrowedBooks.Count());
            Assert.AreEqual(1, account.ActAs<IBorrower>().BorrowedBooks.Single(x => x.BookId == book1.Id).Count);
            Assert.AreEqual(1, account.ActAs<IBorrower>().BorrowedBooks.Single(x => x.BookId == book3.Id).Count);
        }
    }
}
