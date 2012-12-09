using FluentNHibernate.Mapping;

namespace EventSourcing.Sample.NHibernateIntegration
{
    public class NoteEntityMap : ClassMap<NoteEntity>
    {
        public NoteEntityMap()
        {
            Table("EventSourcing_Sample_Note");
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.Title);
            Map(x => x.CreatedTime);
            Map(x => x.UpdatedTime);
        }
    }
}
