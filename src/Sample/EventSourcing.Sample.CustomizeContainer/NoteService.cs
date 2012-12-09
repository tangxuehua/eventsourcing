using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.CustomizeContainer
{
    public interface INoteService
    {
        Note CreateNote(string title);
        void ChangeTitle(Guid id, string title);
    }
    public class NoteService : INoteService
    {
        private IContextManager _contextManager;

        public NoteService(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public Note CreateNote(string title)
        {
            using (var context = _contextManager.GetContext())
            {
                var note = new Note(title);
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
