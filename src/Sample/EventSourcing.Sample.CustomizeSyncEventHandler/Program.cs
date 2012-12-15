using System;
using System.Reflection;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.CustomizeSyncEventHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Configuration.Config("EventSourcing.Sample.CustomizeSyncEventHandler", assembly, assembly);

            Configuration.Instance
                .SyncEventHandlerProvider<InterfaceSyncEventHandlerProvider>()
                .RegisterSyncEventHandlers(assembly);

            var noteService = ObjectContainer.Resolve<INoteService>();
            var note = noteService.CreateNote("Sample Note");
            noteService.ChangeTitle(note.Id, "Updated Note Title");

            Console.Write("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
