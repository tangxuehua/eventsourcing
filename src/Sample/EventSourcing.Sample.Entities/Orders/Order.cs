using System;
using System.Collections.Generic;

namespace EventSourcing.Sample.Entities
{
    public class OrderEntity
    {
        public Guid Id { get; set; }
        public string Customer { get; set; }
        public IList<OrderItemEntity> Items { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
