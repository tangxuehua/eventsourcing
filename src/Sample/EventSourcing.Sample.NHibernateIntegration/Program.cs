using System;
using System.Reflection;
using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.NHibernate;

namespace EventSourcing.Sample.NHibernateIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Configuration
                .Config("EventSourcing.Sample.NHibernateIntegration", assembly, assembly)
                .NHibernate(assembly);

            var noteService = ObjectContainer.Resolve<INoteService>();
            var note = noteService.CreateNote("Sample Note");
            noteService.ChangeTitle(note.Id, "Updated Note Title");

            Console.Write("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
