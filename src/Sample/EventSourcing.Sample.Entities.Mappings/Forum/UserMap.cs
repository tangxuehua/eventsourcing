using EventSourcing.Sample.Entities;
using FluentNHibernate.Mapping;

namespace EventSourcing.Sample.Entities.Mappings
{
    public class UserMap : ClassMap<UserEntity>
    {
        public UserMap()
        {
            Table("EventSourcing_Sample_User");
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.Name);
        }
    }
}
