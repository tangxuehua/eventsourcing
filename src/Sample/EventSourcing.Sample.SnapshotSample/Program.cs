using System;
using System.Reflection;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.SnapshotSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Configuration
                .Config("EventSourcing.Sample.SnapshotSample", assembly, assembly)
                .EnableSnapshot();

            var snapshotter = ObjectContainer.Resolve<ISnapshotter>();
            var contextManager = ObjectContainer.Resolve<IContextManager>();
            var snapshotStore = ObjectContainer.Resolve<ISnapshotStore>();
            var noteService = ObjectContainer.Resolve<INoteService>();

            var note = noteService.CreateNote("Sample Note");
            noteService.ChangeTitle(note.Id, "New Title");

            using (var context = contextManager.GetContext())
            {
                note = context.Load<Note>(note.Id);
                var snapshot = snapshotter.CreateSnapshot(note);
                snapshotStore.StoreShapshot(snapshot);
            }

            noteService.ChangeTitle(note.Id, "New Title2");
            noteService.ChangeTitle(note.Id, "New Title3");

            Console.Write("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
