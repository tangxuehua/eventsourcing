using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.BookBorrowAndReturn
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
