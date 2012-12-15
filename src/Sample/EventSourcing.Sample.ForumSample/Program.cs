using System;
using System.Reflection;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.ForumSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Configuration.Config("EventSourcing.Sample.ForumSample", assembly, assembly);

            var forumService = ObjectContainer.Resolve<IForumService>();
            var userService = ObjectContainer.Resolve<IUserService>();
            var threadService = ObjectContainer.Resolve<IThreadService>();
            var postService = ObjectContainer.Resolve<IPostService>();
            var contextManager = ObjectContainer.Resolve<IContextManager>();

            //创建两个论坛用户
            var user1 = userService.Create("User 1");
            var user2 = userService.Create("User 2");

            //创建一个论坛版块
            var forum = forumService.Create("Sample Forum");

            //论坛版块中发帖
            var thread = threadService.Create("Sample Thread", "Test Body", forum.Id, user1.Id, 100);

            //将帖子设为推荐帖子
            threadService.MarkAsRecommended(thread.Id);

            //将帖子置顶
            threadService.Stick(thread.Id);

            //发表回复1
            var post1 = postService.Create("Sample Post1", thread.Id, user2.Id);
            //发表回复2
            var post2 = postService.Create("Sample Post2", thread.Id, user1.Id);

            Console.Write("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
