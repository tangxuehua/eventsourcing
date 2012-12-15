using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.ForumSample
{
    public class User : AggregateRoot<Guid>
    {
        public string Name { get; private set; }

        public User() { }
        public User(string name) : base(Guid.NewGuid())
        {
            OnEventHappened(new UserCreated(Id, name));
        }
    }

    [SourcableEvent]
    public class UserCreated
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public UserCreated(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
