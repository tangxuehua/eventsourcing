using EventSourcing.Sample.Entities;
using FluentNHibernate.Mapping;

namespace EventSourcing.Sample.Entities.Mappings
{
    public class BalanceChangeHistoryMap : ClassMap<BalanceChangeHistoryEntity>
    {
        public BalanceChangeHistoryMap()
        {
            Table("EventSourcing_Sample_BalanceChangeHistory");
            Id(m => m.Id).GeneratedBy.Assigned();
            Map(x => x.AccountId);
            Map(x => x.ChangeType);
            Map(x => x.Amount);
            Map(x => x.Description).Length(10000);
            Map(x => x.Time);
        }
    }
}
