using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.ForumSample
{
    public class Post : AggregateRoot<Guid>
    {
        public string Body { get; private set; }
        public Guid ThreadId { get; private set; }
        public Guid AuthorId { get; private set; }
        public DateTime CreateTime { get; private set; }

        public Post() { }
        public Post(string body, Thread thread, User author) : base(Guid.NewGuid())
        {
            OnEventHappened(new PostCreated(Id, body, thread.Id, author.Id, DateTime.Now));
        }
    }

    [SourcableEvent]
    public class PostCreated
    {
        public Guid Id { get; private set; }
        public string Body { get; private set; }
        public Guid ThreadId { get; private set; }
        public Guid AuthorId { get; private set; }
        public DateTime CreateTime { get; private set; }

        public PostCreated(Guid id, string body, Guid threadId, Guid authorId, DateTime createTime)
        {
            Id = id;
            Body = body;
            ThreadId = threadId;
            AuthorId = authorId;
            CreateTime = createTime;
        }
    }
}
