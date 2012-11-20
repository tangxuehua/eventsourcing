using EventSourcing.Sample.Entities;
using FluentNHibernate.Mapping;

namespace EventSourcing.Sample.Entities.Mappings
{
    public class OrderItemMap : ClassMap<OrderItemEntity>
    {
        public OrderItemMap()
        {
            Table("EventSourcing_Sample_OrderItem");
            CompositeId()
                .KeyProperty(x => x.OrderId)
                .KeyProperty(x => x.ProductId);
            Map(x => x.Price);
            Map(x => x.Amount);
        }
    }
}
