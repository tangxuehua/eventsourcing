using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.NHibernate;
using EventSourcing.Sample.Entities;
using EventSourcing.Sample.Model.Orders;

namespace EventSourcing.Sample.EventSubscribers
{
    public class ProductEventSubscriber
    {
        private ISessionHelper _sessionHelper;

        public ProductEventSubscriber(ISessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
        }

        [AsyncEventHandler]
        public void Handle(ProductCreated evnt)
        {
            _sessionHelper.ExecuteAction((session) =>
            {
                session.Save(ObjectHelper.CreateObject<ProductEntity>(evnt));
            });
        }
    }
}
