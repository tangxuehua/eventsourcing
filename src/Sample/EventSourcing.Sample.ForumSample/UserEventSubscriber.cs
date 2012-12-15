using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.ForumSample
{
    public class UserEventSubscriber
    {
        private IDbConnectionFactory _connectionFactory;

        public UserEventSubscriber(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [AsyncEventHandler]
        private void Handle(UserCreated evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                conn.Insert(evnt, "EventSourcing_Sample_User");
            }
        }
    }
}
