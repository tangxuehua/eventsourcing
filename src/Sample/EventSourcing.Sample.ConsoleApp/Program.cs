using System;
using System.Reflection;
using CodeSharp.EventSourcing;
using EventSourcing.Sample.Application;
using EventSourcing.Sample.Model.BookBorrowAndReturn;

namespace EventSourcing.Sample.ConsoleApp
{
    class Program
    {
        private static Random _random = new Random();

        static void Main(string[] args)
        {
            InitializeFramework();
            Console.WriteLine("Framework initialized, press enter to start test.");
            Console.ReadLine();
            BookSampeTest();
            Console.WriteLine("Test finished, press enter to exit.");
            Console.ReadLine();
        }

        private static void BookSampeTest()
        {
            var accountService = ObjectContainer.Resolve<ILibraryAccountService>();
            var libraryService = ObjectContainer.Resolve<ILibraryService>();
            var bookService = ObjectContainer.Resolve<IBookService>();

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
        }
        private static void InitializeFramework()
        {
            var modelAssembly = Assembly.Load("EventSourcing.Sample.Model");
            var applicationAssembly = Assembly.Load("EventSourcing.Sample.Application");
            var eventSubscriberAssembly = Assembly.Load("EventSourcing.Sample.EventSubscribers");
            var assemblies = new Assembly[] { modelAssembly, applicationAssembly, eventSubscriberAssembly };
            var configAssembly = Assembly.GetExecutingAssembly();
            Configuration.Config("EventSourcing.Sample.Test", configAssembly, assemblies);
        }
        private static string RandomString()
        {
            return "EventSourcing_" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Ticks + _random.Next(100);
        }
    }
}
