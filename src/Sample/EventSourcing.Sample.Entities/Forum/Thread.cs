using System;

namespace EventSourcing.Sample.Entities
{
    public class ThreadEntity
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Guid ForumId { get; set; }
        public Guid AuthorId { get; set; }
        public int Marks { get; set; }
        public int Status { get; set; }
        public bool IsStick { get; set; }
        public DateTime? StickDate { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
