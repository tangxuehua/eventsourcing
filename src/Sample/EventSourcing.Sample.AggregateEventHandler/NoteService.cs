using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.AggregateEventHandler
{
    public interface INoteService
    {
        Note CreateNote(Guid bookId, string title);
        void ChangeTitle(Guid id, string title);
    }
    public class NoteService : INoteService
    {
        private IContextManager _contextManager;

        public NoteService(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public Note CreateNote(Guid bookId, string title)
        {
            using (var context = _contextManager.GetContext())
            {
                var book = context.Load<NoteBook>(bookId);
                var note = new Note(book, title);
                context.Add(note);
                context.SaveChanges();
                return note;
            }
        }

        public void ChangeTitle(Guid id, string title)
        {
            using (var context = _contextManager.GetContext())
            {
                var note = context.Load<Note>(id);
                note.ChangeTitle(title);
                context.SaveChanges();
            }
        }
    }
}
