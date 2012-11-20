using System;

namespace EventSourcing.Sample.Entities
{
    public class PostEntity
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public Guid ThreadId { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
