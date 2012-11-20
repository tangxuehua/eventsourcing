using EventSourcing.Sample.Entities;
using FluentNHibernate.Mapping;

namespace EventSourcing.Sample.Entities.Mappings
{
    public class LibraryMap : ClassMap<LibraryEntity>
    {
        public LibraryMap()
        {
            Table("EventSourcing_Sample_Library");
            Id(m => m.Id).GeneratedBy.Assigned();
            Map(x => x.Name).Length(512);
            HasMany(x => x.BookStoreItems).KeyColumn("LibraryId");
        }
    }
}
