using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.NHibernate;
using EventSourcing.Sample.Entities;
using EventSourcing.Sample.Model.MoneyTransfer;

namespace EventSourcing.Sample.EventSubscribers
{
    public class BankAccountEventSubscriber
    {
        private ISessionHelper _sessionHelper;

        public BankAccountEventSubscriber(ISessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
        }

        [AsyncEventHandler]
        private void Handle(BankAccountCreated evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                session.Save(ObjectHelper.CreateObject<BankAccountEntity>(evnt));
            });
        }
        [AsyncEventHandler]
        private void Handle(AccountBalanceUpdated evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                var bankAccount = session.Get<BankAccountEntity>(evnt.BankAccountId);
                bankAccount.Balance = evnt.Balance;
                session.Update(bankAccount);
            });
        }
    }
}
