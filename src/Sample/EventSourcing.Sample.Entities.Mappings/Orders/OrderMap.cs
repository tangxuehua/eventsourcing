using EventSourcing.Sample.Entities;
using FluentNHibernate.Mapping;

namespace EventSourcing.Sample.Entities.Mappings
{
    public class OrderMap : ClassMap<OrderEntity>
    {
        public OrderMap()
        {
            Table("EventSourcing_Sample_Order");
            Id(m => m.Id).GeneratedBy.Assigned();
            Map(x => x.Customer).Length(256);
            HasMany(m => m.Items).KeyColumn("OrderId");
            Map(x => x.CreatedTime);
        }
    }
}
