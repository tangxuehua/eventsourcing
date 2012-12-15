using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.NotifyEventSample
{
    public class Product : AggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public double Price { get; private set; }

        public Product() { }
        public Product(string name, string description, double price) : base(Guid.NewGuid())
        {
            OnEventHappened(new ProductCreated(Id, name, description, price));
        }
    }
    [SourcableEvent]
    public class ProductCreated
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public double Price { get; private set; }

        public ProductCreated(Guid id, string name, string description, double price)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
        }
    }
}
