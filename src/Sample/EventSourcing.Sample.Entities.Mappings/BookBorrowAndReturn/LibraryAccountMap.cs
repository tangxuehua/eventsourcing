using EventSourcing.Sample.Entities;
using FluentNHibernate.Mapping;

namespace EventSourcing.Sample.Entities.Mappings
{
    public class LibraryAccountMap : ClassMap<LibraryAccountEntity>
    {
        public LibraryAccountMap()
        {
            Table("EventSourcing_Sample_LibraryAccount");
            Id(m => m.Id).GeneratedBy.Assigned();
            Map(x => x.Number).Length(128);
            Map(x => x.Owner).Length(256);
            HasMany(x => x.BorrowedBooks).KeyColumn("AccountId");
        }
    }
}
