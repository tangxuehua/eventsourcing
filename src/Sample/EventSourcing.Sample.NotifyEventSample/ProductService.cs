using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.NotifyEventSample
{
    public interface IProductService
    {
        Product Create(string name, string description, double price);
    }

    public class ProductService : IProductService
    {
        private IContextManager _contextManager;

        public ProductService(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public Product Create(string name, string description, double price)
        {
            using (var context = _contextManager.GetContext())
            {
                var product = new Product(name, description, price);
                context.Add(product);
                context.SaveChanges();
                return product;
            }
        }
    }
}
