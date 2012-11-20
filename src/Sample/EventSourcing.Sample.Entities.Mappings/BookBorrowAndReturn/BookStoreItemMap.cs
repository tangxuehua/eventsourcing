using EventSourcing.Sample.Entities;
using FluentNHibernate.Mapping;

namespace EventSourcing.Sample.Entities.Mappings
{
    public class BookStoreItemMap : ClassMap<BookStoreItemEntity>
    {
        public BookStoreItemMap()
        {
            Table("EventSourcing_Sample_BookStoreItem");
            CompositeId()
                .KeyProperty(x => x.LibraryId)
                .KeyProperty(x => x.BookId);
            Map(x => x.Count);
        }
    }
}
