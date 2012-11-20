using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.NHibernate;
using EventSourcing.Sample.Entities;
using EventSourcing.Sample.Model.MoneyTransfer;

namespace EventSourcing.Sample.EventSubscribers
{
    public class BalanceChangeHistoryEventSubscriber
    {
        private ISessionHelper _sessionHelper;

        public BalanceChangeHistoryEventSubscriber(ISessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
        }

        [AsyncEventHandler]
        private void Handle(BalanceChangeHistoryCreated evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                session.Save(ObjectHelper.CreateObject<BalanceChangeHistoryEntity>(evnt));
            });
        }
    }
}
