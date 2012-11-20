using FluentNHibernate.Mapping;
using EventSourcing.Sample.Entities;

namespace EventSourcing.Sample.Entities.Mappings
{
    public class BankAccountMap : ClassMap<BankAccountEntity>
    {
        public BankAccountMap()
        {
            Table("EventSourcing_Sample_BankAccount");
            Id(m => m.Id).GeneratedBy.Assigned();
            Map(x => x.AccountNumber).Length(128);
            Map(x => x.Customer).Length(256);
            Map(x => x.Balance);
        }
    }
}
