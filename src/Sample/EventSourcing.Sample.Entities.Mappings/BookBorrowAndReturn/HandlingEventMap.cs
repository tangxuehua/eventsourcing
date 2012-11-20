using EventSourcing.Sample.Entities;
using FluentNHibernate.Mapping;

namespace EventSourcing.Sample.Entities.Mappings
{
    public class HandlingEventMap : ClassMap<HandlingEventEntity>
    {
        public HandlingEventMap()
        {
            Table("EventSourcing_Sample_HandlingEvent");
            Id(m => m.Id).GeneratedBy.Assigned();
            Map(x => x.BookId);
            Map(x => x.AccountId);
            Map(x => x.HandlingType).CustomType<HandlingType>();
            Map(x => x.Time);
        }
    }
}
