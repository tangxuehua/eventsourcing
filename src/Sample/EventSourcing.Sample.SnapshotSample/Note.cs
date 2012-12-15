using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.SnapshotSample
{
    public class Note : AggregateRoot<Guid>, ISnapshotable<NoteSnapshot>
    {
        public string Title { get; private set; }
        public DateTime CreatedTime { get; private set; }
        public DateTime UpdatedTime { get; private set; }

        public Note() { }
        public Note(string title) : base(Guid.NewGuid())
        {
            var currentTime = DateTime.Now;
            OnEventHappened(new NoteCreated(Id, title, currentTime, currentTime));
        }

        public void ChangeTitle(string title)
        {
            OnEventHappened(new NoteTitleChanged(Id, title, DateTime.Now));
        }

        public NoteSnapshot CreateSnapshot()
        {
            return new NoteSnapshot {
                Id = Id,
                Title = Title,
                CreatedTime = CreatedTime,
                UpdatedTime = UpdatedTime
            };
        }
        public void RestoreFromSnapshot(NoteSnapshot snapshot)
        {
            Title = snapshot.Title;
            CreatedTime = snapshot.CreatedTime;
            UpdatedTime = snapshot.UpdatedTime;
        }
    }

    [SourcableEvent]
    public class NoteCreated
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public DateTime CreatedTime { get; private set; }
        public DateTime UpdatedTime { get; private set; }

        public NoteCreated(Guid id, string title, DateTime createdTime, DateTime updatedTime)
        {
            Id = id;
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
    [Snapshot]
    public class NoteSnapshot
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
