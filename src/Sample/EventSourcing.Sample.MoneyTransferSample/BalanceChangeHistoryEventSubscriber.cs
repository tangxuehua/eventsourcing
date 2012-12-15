using System.Data;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.MoneyTransferSample
{
    public class BalanceChangeHistoryEventSubscriber
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public BalanceChangeHistoryEventSubscriber(ICurrentDbTransactionProvider transactionProvider)
        {
            _transaction = transactionProvider.CurrentTransaction;
            _connection = _transaction.Connection;
        }

        [SyncEventHandler]
        private void Handle(BalanceChangeHistoryCreated evnt)
        {
            _connection.Insert(evnt, "EventSourcing_Sample_BalanceChangeHistory", _transaction);
        }
    }
}
