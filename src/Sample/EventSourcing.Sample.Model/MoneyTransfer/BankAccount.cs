using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.Model.MoneyTransfer
{
    public class BankAccount : AggregateRoot<Guid>
    {
        public string AccountNumber { get; private set; }
        public string Customer { get; private set; }
        public double Balance { get; private set; }

        public BankAccount()
        {
        }
        public BankAccount(string customer, string accountNumber) : base(Guid.NewGuid())
        {
            Assert.IsNotNullOrWhiteSpace(customer);
            Assert.IsNotNullOrWhiteSpace(accountNumber);
            OnEventHappened(new BankAccountCreated(Id, customer, accountNumber));
        }

        public void Deposit(double amount)
        {
            Assert.Greater(amount, 0);
            OnEventHappened(new AccountBalanceUpdated(Id, Balance + amount));
            OnAggregateRootCreated(new BalanceChangeHistory(this, ChangeType.Deposite, amount, "存入", DateTime.Now));
        }
        public void Withdraw(double amount)
        {
            Assert.GreaterOrEqual(Balance, amount);
            OnEventHappened(new AccountBalanceUpdated(Id, Balance - amount));
            OnAggregateRootCreated(new BalanceChangeHistory(this, ChangeType.Withdraw, amount, "取现", DateTime.Now));
        }
        public void TransferOut(BankAccount targetAccount, double amount)
        {
            Assert.GreaterOrEqual(Balance, amount);
            OnEventHappened(new AccountBalanceUpdated(Id, Balance - amount));
            OnAggregateRootCreated(new BalanceChangeHistory(this, ChangeType.TransferOut, amount, string.Format("资金转账，类型：转出，对方账号：{0}，转出金额:{1}", targetAccount.AccountNumber, amount), DateTime.Now));
        }
        public void TransferIn(BankAccount sourceAccount, double amount)
        {
            Assert.Greater(amount, 0);
            OnEventHappened(new AccountBalanceUpdated(Id, Balance + amount));
            OnAggregateRootCreated(new BalanceChangeHistory(this, ChangeType.TransferIn, amount, string.Format("资金转账，类型：转入，对方账号：{0}，转入金额:{1}", sourceAccount.AccountNumber, amount), DateTime.Now));
        }
    }
    [SourcableEvent]
    public class BankAccountCreated
    {
        public Guid Id { get; private set; }
        public string AccountNumber { get; private set; }
        public string Customer { get; private set; }

        public BankAccountCreated(Guid bankAccountId, string customer, string accountNumber)
        {
            Id = bankAccountId;
            Customer = customer;
            AccountNumber = accountNumber;
        }
    }
    [SourcableEvent]
    public class AccountBalanceUpdated
    {
        public Guid BankAccountId { get; private set; }
        public double Balance { get; private set; }

        public AccountBalanceUpdated(Guid bankAccountId, double amount)
        {
            BankAccountId = bankAccountId;
            Balance = amount;
        }
    }
}
