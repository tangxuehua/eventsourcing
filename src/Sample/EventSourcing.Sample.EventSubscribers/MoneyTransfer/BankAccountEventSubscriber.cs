using System.Data;
using CodeSharp.EventSourcing;
using EventSourcing.Sample.Model.MoneyTransfer;

namespace EventSourcing.Sample.EventSubscribers
{
    public class BankAccountEventSubscriber
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public BankAccountEventSubscriber(ICurrentDbTransactionProvider transactionProvider)
        {
            _transaction = transactionProvider.CurrentTransaction;
            _connection = _transaction.Connection;
        }

        [SyncEventHandler]
        private void Handle(BankAccountCreated evnt)
        {
            _connection.Insert(
                new
                {
                    Id = evnt.Id,
                    AccountNumber = evnt.AccountNumber,
                    Customer = evnt.Customer,
                    Balance = 0
                },
                "EventSourcing_Sample_BankAccount", _transaction);
        }
        [SyncEventHandler]
        private void Handle(AccountBalanceUpdated evnt)
        {
            _connection.Update(
                new { Balance = evnt.Balance },
                new { Id = evnt.BankAccountId },
                "EventSourcing_Sample_BankAccount", _transaction);
        }
    }
}
