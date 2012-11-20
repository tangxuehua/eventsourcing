using EventSourcing.Sample.Entities;
using FluentNHibernate.Mapping;

namespace EventSourcing.Sample.Entities.Mappings
{
    public class PostMap : ClassMap<PostEntity>
    {
        public PostMap()
        {
            Table("EventSourcing_Sample_Post");
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.Body);
            Map(x => x.ThreadId);
            Map(x => x.AuthorId);
            Map(x => x.CreateTime);
        }
    }
}
