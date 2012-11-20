using System;

namespace EventSourcing.Sample.Entities
{
    public class ForumEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int TotalThread { get; set; }
        public Guid? LatestThreadId { get; set; }
        public int TotalPost { get; set; }
        public Guid? LatestPostAuthorId { get; set; }
    }
}
