using EventSourcing.Sample.Entities;
using FluentNHibernate.Mapping;

namespace EventSourcing.Sample.Entities.Mappings
{
    public class BookMap : ClassMap<BookEntity>
    {
        public BookMap()
        {
            Table("EventSourcing_Sample_Book");
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.Name);
            Map(x => x.ISBN);
            Map(x => x.Author);
            Map(x => x.Publisher);
            Map(x => x.Description);
        }
    }
}
