using EventSourcing.Sample.Entities;
using FluentNHibernate.Mapping;

namespace EventSourcing.Sample.Entities.Mappings
{
    public class BorrowedBookMap : ClassMap<BorrowedBookEntity>
    {
        public BorrowedBookMap()
        {
            Table("EventSourcing_Sample_BorrowedBook");
            CompositeId()
                .KeyProperty(x => x.AccountId)
                .KeyProperty(x => x.BookId);
            Map(x => x.Count);
        }
    }
}
