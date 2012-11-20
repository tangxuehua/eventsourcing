using CodeSharp.EventSourcing;
using EventSourcing.Sample.Model.BookBorrowAndReturn;

namespace EventSourcing.Sample.Application
{
    public interface ILibraryService
    {
        Library Create(string name);
    }

    public class LibraryService : ILibraryService
    {
        private IContextManager _contextManager;

        public LibraryService(IContextManager contextManger)
        {
            _contextManager = contextManger;
        }

        public Library Create(string name)
        {
            using (var context = _contextManager.GetContext())
            {
                var library = new Library(name);
                context.Add(library);
                context.SaveChanges();
                return library;
            }
        }
    }
}
