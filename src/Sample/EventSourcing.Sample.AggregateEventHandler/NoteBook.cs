using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.AggregateEventHandler
{
    public class NoteBook : AggregateRoot<Guid>
    {
        public string Title { get; private set; }
        public int TotalNoteCount { get; private set; }

        public NoteBook() { }
        public NoteBook(string title) : base(Guid.NewGuid())
        {
            var currentTime = DateTime.Now;
            OnEventHappened(new NoteBookCreated(Id, title));
        }

        [AggregateEventHandler("BookId")]
        public void Handle(NoteCreated evnt)
        {
            OnEventHappened(new TotalNoteCountChanged(Id, TotalNoteCount + 1));
        }
    }

    [SourcableEvent]
    public class NoteBookCreated
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }

        public NoteBookCreated(Guid id, string title)
        {
            Id = id;
            Title = title;
        }
    }
    [SourcableEvent]
    public class TotalNoteCountChanged
    {
        public Guid BookId { get; private set; }
        public int TotalNoteCount { get; private set; }

        public TotalNoteCountChanged(Guid bookId, int totalNoteCount)
        {
            BookId = bookId;
            TotalNoteCount = totalNoteCount;
        }
    }
}
