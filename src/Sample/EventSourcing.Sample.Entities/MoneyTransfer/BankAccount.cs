using System;

namespace EventSourcing.Sample.Entities
{
    public class BankAccountEntity
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public string Customer { get; set; }
        public double Balance { get; set; }
    }
}
