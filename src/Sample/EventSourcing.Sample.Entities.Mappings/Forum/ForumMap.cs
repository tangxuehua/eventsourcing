using EventSourcing.Sample.Entities;
using FluentNHibernate.Mapping;

namespace EventSourcing.Sample.Entities.Mappings
{
    public class ForumMap : ClassMap<ForumEntity>
    {
        public ForumMap()
        {
            Table("EventSourcing_Sample_Forum");
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.Name);
            Map(x => x.TotalThread);
            Map(x => x.TotalPost);
            Map(x => x.LatestThreadId);
            Map(x => x.LatestPostAuthorId);
        }
    }
}
