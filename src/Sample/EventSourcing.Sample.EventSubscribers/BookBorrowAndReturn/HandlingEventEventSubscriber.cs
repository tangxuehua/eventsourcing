using CodeSharp.EventSourcing;
using EventSourcing.Sample.Model.BookBorrowAndReturn;

namespace EventSourcing.Sample.EventSubscribers
{
    public class HandlingEventEventSubscriber
    {
        private IDbConnectionFactory _connectionFactory;

        public HandlingEventEventSubscriber(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [AsyncEventHandler]
        private void Handle(HandlingEventCreated evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                conn.Insert(evnt, "EventSourcing_Sample_HandlingEvent");
            }
        }
    }
}
