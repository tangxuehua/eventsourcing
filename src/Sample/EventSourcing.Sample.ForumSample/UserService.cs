using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.ForumSample
{
    public interface IUserService
    {
        User Create(string name);
    }
    public class UserService : IUserService
    {
        private IContextManager _contextManager;

        public UserService(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public User Create(string name)
        {
            using (var context = _contextManager.GetContext())
            {
                var user = new User(name);
                context.Add(user);
                context.SaveChanges();
                return user;
            }
        }
    }
}
