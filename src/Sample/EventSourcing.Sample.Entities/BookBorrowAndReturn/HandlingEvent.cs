using System;

namespace EventSourcing.Sample.Entities
{
    public class HandlingEventEntity
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public Guid AccountId { get; set; }
        public Guid LibraryId { get; set; }
        public HandlingType HandlingType { get; set; }
        public DateTime Time { get; set; }
    }
    public enum HandlingType
    {
        Borrow = 1,
        Return
    }
}
