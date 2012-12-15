using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.ForumSample
{
    public class ForumEventSubscriber
    {
        private IDbConnectionFactory _connectionFactory;

        public ForumEventSubscriber(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [AsyncEventHandler]
        private void Handle(ForumCreated evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                conn.Insert(new
                {
                    Id = evnt.Id,
                    Name = evnt.Name,
                    TotalThread = evnt.State.TotalThread,
                    TotalPost = evnt.State.TotalPost,
                    LatestThreadId = evnt.State.LatestThreadId,
                    LatestPostAuthorId = evnt.State.LatestPostAuthorId
                }, "EventSourcing_Sample_Forum");
            }
        }
        [AsyncEventHandler]
        private void Handle(ForumStateChanged evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                conn.Update(new
                {
                    TotalThread = evnt.State.TotalThread,
                    TotalPost = evnt.State.TotalPost,
                    LatestThreadId = evnt.State.LatestThreadId,
                    LatestPostAuthorId = evnt.State.LatestPostAuthorId
                },
                new { Id = evnt.Id },
                "EventSourcing_Sample_Forum");
            }
        }
    }
}
