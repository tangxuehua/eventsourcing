using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.AggregateEventHandler
{
    public interface INoteBookService
    {
        NoteBook CreateBook(string title);
    }
    public class NoteBookService : INoteBookService
    {
        private IContextManager _contextManager;

        public NoteBookService(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public NoteBook CreateBook(string title)
        {
            using (var context = _contextManager.GetContext())
            {
                var book = new NoteBook(title);
                context.Add(book);
                context.SaveChanges();
                return book;
            }
        }
    }
}
