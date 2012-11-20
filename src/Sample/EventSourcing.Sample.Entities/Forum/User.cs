using System;

namespace EventSourcing.Sample.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
