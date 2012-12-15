using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.ForumSample
{
    public class ThreadEventSubscriber
    {
        private IDbConnectionFactory _connectionFactory;

        public ThreadEventSubscriber(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [AsyncEventHandler]
        private void Handle(ThreadCreated evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                conn.Insert(
                    new
                    {
                        Id = evnt.Id,
                        Subject = evnt.Subject,
                        Body = evnt.Body,
                        ForumId = evnt.ForumId,
                        AuthorId = evnt.AuthorId,
                        Marks = evnt.Marks,
                        Status = evnt.Status,
                        CreateTime = evnt.CreateTime,
                        IsStick = evnt.StickInfo.IsStick,
                        StickDate = evnt.StickInfo.StickDate
                    }, "EventSourcing_Sample_Thread");
            }
        }
        [AsyncEventHandler]
        private void Handle(ContentChanged evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                conn.Update(
                    new { Subject = evnt.Subject, Body = evnt.Body, Marks = evnt.Marks },
                    new { Id = evnt.Id },
                    "EventSourcing_Sample_Thread");
            }
        }
        [AsyncEventHandler]
        private void Handle(ThreadStatusChanged evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                conn.Update(
                    new { Status = evnt.Status, },
                    new { Id = evnt.Id },
                    "EventSourcing_Sample_Thread");
            }
        }
        [AsyncEventHandler]
        private void Handle(ThreadStickInfoChanged evnt)
        {
            using (var conn = _connectionFactory.OpenConnection())
            {
                conn.Update(
                    new { IsStick = evnt.StickInfo.IsStick, StickDate = evnt.StickInfo.StickDate },
                    new { Id = evnt.Id },
                    "EventSourcing_Sample_Thread");
            }
        }
    }
}
