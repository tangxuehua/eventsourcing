using System;
using System.Reflection;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.CustomizeConfigurationInitializer
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Configuration.Config("EventSourcing.Sample.CustomizeConfigurationInitializer", new SimpleConfigurationInitializer(), assembly);

            var noteService = ObjectContainer.Resolve<INoteService>();
            var note = noteService.CreateNote("Sample Note");
            noteService.ChangeTitle(note.Id, "Updated Note Title");

            Console.Write("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
