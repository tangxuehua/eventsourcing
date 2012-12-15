using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.AggregateEventHandler
{
    public class Note : AggregateRoot<Guid>
    {
        public string Title { get; private set; }
        public Guid BookId { get; private set; }
        public DateTime CreatedTime { get; private set; }
        public DateTime UpdatedTime { get; private set; }

        public Note() { }
        public Note(NoteBook noteBook, string title) : base(Guid.NewGuid())
        {
            var currentTime = DateTime.Now;
            OnEventHappened(new NoteCreated(Id, noteBook.Id, title, currentTime, currentTime));
        }

        public void ChangeTitle(string title)
        {
            OnEventHappened(new NoteTitleChanged(Id, title, DateTime.Now));
        }
    }

    [SourcableEvent]
    public class NoteCreated
    {
        public Guid Id { get; private set; }
        public Guid BookId { get; private set; }
        public string Title { get; private set; }
        public DateTime CreatedTime { get; private set; }
        public DateTime UpdatedTime { get; private set; }

        public NoteCreated(Guid id, Guid bookId, string title, DateTime createdTime, DateTime updatedTime)
        {
            Id = id;
            BookId = bookId;
            Title = title;
            CreatedTime = createdTime;
            UpdatedTime = updatedTime;
        }
    }
    [SourcableEvent]
    public class NoteTitleChanged
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public DateTime UpdatedTime { get; private set; }

        public NoteTitleChanged(Guid id, string title, DateTime updatedTime)
        {
            Id = id;
            Title = title;
            UpdatedTime = updatedTime;
        }
    }
}
