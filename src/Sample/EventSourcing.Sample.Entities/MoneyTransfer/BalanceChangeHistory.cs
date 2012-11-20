using System;

namespace EventSourcing.Sample.Entities
{
    public class BalanceChangeHistoryEntity
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public int ChangeType { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime Time { get; set; }
    }
}
