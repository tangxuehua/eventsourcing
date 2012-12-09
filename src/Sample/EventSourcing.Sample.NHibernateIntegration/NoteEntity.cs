using System;

namespace EventSourcing.Sample.NHibernateIntegration
{
    public class NoteEntity
    {
        public virtual Guid Id { get; set; }
        public virtual string Title { get; set; }
        public virtual DateTime CreatedTime { get; set; }
        public virtual DateTime UpdatedTime { get; set; }
    }
}
