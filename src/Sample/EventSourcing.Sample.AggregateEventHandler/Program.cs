using System;
using System.Reflection;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.AggregateEventHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Configuration.Config("EventSourcing.Sample.AggregateEventHandler", assembly, assembly);

            var noteBookService = ObjectContainer.Resolve<INoteBookService>();
            var noteService = ObjectContainer.Resolve<INoteService>();
            var book = noteBookService.CreateBook("Sample Book");
            noteService.CreateNote(book.Id, "Sample Note1");
            noteService.CreateNote(book.Id, "Sample Note2");

            Console.Write("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
