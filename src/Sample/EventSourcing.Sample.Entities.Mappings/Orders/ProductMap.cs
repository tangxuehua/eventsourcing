using EventSourcing.Sample.Entities;
using FluentNHibernate.Mapping;

namespace EventSourcing.Sample.Entities.Mappings
{
    public class ProductMap : ClassMap<ProductEntity>
    {
        public ProductMap()
        {
            Table("EventSourcing_Sample_Product");
            Id(m => m.Id).GeneratedBy.Assigned();
            Map(x => x.Name).Length(256);
            Map(x => x.Description).Length(10000);
            Map(x => x.Price);
        }
    }
}
