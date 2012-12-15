using System;
using System.Collections.Generic;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.ForumSample
{
    public class Forum : AggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public ForumState State { get; private set; }

        public Forum() { }
        public Forum(string name) : base(Guid.NewGuid())
        {
            OnEventHappened(new ForumCreated(Id, name, new ForumState()));
        }

        [AggregateEventHandler("ForumId")]
        private void ChangeState(ThreadCreated threadCreated)
        {
            var state = this.State.Clone(new
            {
                TotalThread = this.State.TotalThread + 1,
                LatestThreadId = threadCreated.Id
            });
            OnEventHappened(new ForumStateChanged(Id, state));
        }
        [AggregateEventHandler("Thread:ThreadId", "Forum:ForumId")]
        private void ChangeState(PostCreated postCreated)
        {
            var state = this.State.Clone(new
            {
                TotalPost = this.State.TotalPost + 1,
                LatestPostAuthorId = postCreated.AuthorId
            });
            OnEventHappened(new ForumStateChanged(Id, state));
        }
    }
    public class ForumState : ValueObject<ForumState>
    {
        public int TotalThread { get; private set; }
        public Guid? LatestThreadId { get; private set; }
        public int TotalPost { get; private set; }
        public Guid? LatestPostAuthorId { get; private set; }

        public ForumState() { }
        public ForumState(int totalThread, Guid latestThreadId, int totalPost, Guid latestPostAuthorId)
        {
            TotalThread = totalThread;
            LatestThreadId = latestThreadId;
            TotalPost = totalPost;
            LatestPostAuthorId = latestPostAuthorId;
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return TotalThread;
            yield return LatestThreadId;
            yield return TotalPost;
            yield return LatestPostAuthorId;
        }
    }

    [SourcableEvent]
    public class ForumCreated
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public ForumState State { get; private set; }

        public ForumCreated(Guid id, string name, ForumState state)
        {
            Id = id;
            Name = name;
            State = state;
        }
    }
    [SourcableEvent]
    public class ForumStateChanged
    {
        public Guid Id { get; private set; }
        public ForumState State { get; private set; }

        public ForumStateChanged(Guid id, ForumState state)
        {
            Id = id;
            State = state;
        }
    }
}
