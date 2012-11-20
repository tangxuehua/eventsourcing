using System;

namespace EventSourcing.Sample.Entities
{
    public class BookEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
    }
}
