using System;
using System.Reflection;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.BookBorrowAndReturn
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Configuration.Config("EventSourcing.Sample.BookBorrowAndReturn", assembly, assembly);

            var accountService = ObjectContainer.Resolve<ILibraryAccountService>();
            var libraryService = ObjectContainer.Resolve<ILibraryService>();
            var bookService = ObjectContainer.Resolve<IBookService>();

            //创建借书账号
            var account = accountService.Create("00001", "tangxuehua");

            //创建三本书
            var book1Info = new BookInfo("Java Programming", "ISBN-001", "John", "Publisher1", string.Empty);
            var book2Info = new BookInfo(".Net Programming", "ISBN-002", "Jim", "Publisher2", string.Empty);
            var book3Info = new BookInfo("Mono Develop", "ISBN-003", "Richer", "Publisher3", string.Empty);
            var book1 = bookService.CreateBook(book1Info);
            var book2 = bookService.CreateBook(book2Info);
            var book3 = bookService.CreateBook(book3Info);

            //创建图书馆
            var library = libraryService.Create("Sample Book Library");

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

            Console.Write("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
