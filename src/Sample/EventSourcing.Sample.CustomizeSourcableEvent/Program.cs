using System;
using System.Reflection;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.CustomizeSourcableEvent
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Configuration.Config("EventSourcing.Sample.CustomizeSourcableEvent", assembly, assembly);

            //告诉框架实现了ISourcableEvent接口的类是可溯源事件
            var eventTypeProvider = ObjectContainer.Resolve<IEventTypeProvider>() as DefaultEventTypeProvider;
            eventTypeProvider.SourcableEventInterfaceType = typeof(ISourcableEvent);
            ObjectContainer.Resolve<ITypeNameMappingProvider>().RegisterMappings(NameTypeMappingType.SourcableEventMapping, assembly);

            var noteService = ObjectContainer.Resolve<INoteService>();
            var note = noteService.CreateNote("Sample Note");
            noteService.ChangeTitle(note.Id, "Updated Note Title");

            Console.Write("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
