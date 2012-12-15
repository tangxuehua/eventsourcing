using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.ForumSample
{
    public class PostEventSubscriber
    {
        private IDbConnectionFactory _connectionFactory;

        public PostEventSubscriber(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [AsyncEventHandler]
        private void Handle(PostCreated evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                conn.Insert(evnt, "EventSourcing_Sample_Post");
            }
        }
    }
}
