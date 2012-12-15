using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.MoneyTransferSample
{
    public class BalanceChangeHistory : AggregateRoot<Guid>
    {
        public BalanceChangeHistory() { }
        public BalanceChangeHistory(BankAccount account, ChangeType changeType, double amount, string description, DateTime time) : base(Guid.NewGuid())
        {
            OnEventHappened(new BalanceChangeHistoryCreated(Id, account.Id, changeType, amount, description, DateTime.Now));
        }

        public Guid AccountId { get; private set; }
        public ChangeType ChangeType { get; private set; }
        public double Amount { get; private set; }
        public string Description { get; private set; }
        public DateTime Time { get; private set; }
    }
    public enum ChangeType
    {
        Deposite = 1,
        Withdraw,
        TransferOut,
        TransferIn
    }
    [SourcableEvent]
    public class BalanceChangeHistoryCreated
    {
        public Guid Id { get; private set; }
        public Guid AccountId { get; private set; }
        public ChangeType ChangeType { get; private set; }
        public double Amount { get; private set; }
        public string Description { get; private set; }
        public DateTime Time { get; private set; }

        public BalanceChangeHistoryCreated(Guid id, Guid accountId, ChangeType changeType, double amount, string description, DateTime time)
        {
            Id = id;
            AccountId = accountId;
            ChangeType = changeType;
            Amount = amount;
            Description = description;
            Time = time;
        }
    }
}
