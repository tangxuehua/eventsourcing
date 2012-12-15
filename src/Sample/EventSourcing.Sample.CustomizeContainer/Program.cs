using System;
using System.Reflection;
using Castle.Windsor;
using CodeSharp.EventSourcing;
using CodeSharp.EventSourcing.Container.Windsor;
using CodeSharp.EventSourcing.Container.StructureMap;
using CodeSharp.EventSourcing.Container.Unity;
using Microsoft.Practices.Unity;

namespace EventSourcing.Sample.CustomizeContainer
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();

            //Use Castle Windsor Container
            //Configuration.Config("EventSourcing.Sample.CustomizeContainer", assembly, new WindsorObjectContainer(new WindsorContainer()), assembly);

            //Use StructureMap Container
            //Configuration.Config("EventSourcing.Sample.CustomizeContainer", assembly, new StructureMapObjectContainer(), assembly);

            //Use Unity Container
            Configuration.Config("EventSourcing.Sample.CustomizeContainer", assembly, new UnityObjectContainer(new UnityContainer()), assembly);

            var noteService = ObjectContainer.Resolve<INoteService>();
            var note = noteService.CreateNote("Sample Note");
            noteService.ChangeTitle(note.Id, "Updated Note Title");

            Console.Write("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
