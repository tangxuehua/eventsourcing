using EventSourcing.Sample.Entities;
using FluentNHibernate.Mapping;

namespace EventSourcing.Sample.Entities.Mappings
{
    public class ThreadMap : ClassMap<ThreadEntity>
    {
        public ThreadMap()
        {
            Table("EventSourcing_Sample_Thread");
            Id(m => m.Id).GeneratedBy.Assigned();
            Map(x => x.Subject);
            Map(x => x.Body);
            Map(x => x.ForumId);
            Map(x => x.AuthorId);
            Map(x => x.Status);
            Map(x => x.Marks);
            Map(x => x.CreateTime);
            Map(x => x.IsStick);
            Map(x => x.StickDate);
        }
    }
}
