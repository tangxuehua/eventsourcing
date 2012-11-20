using System;

namespace EventSourcing.Sample.Entities
{
    public class OrderItemEntity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }

        public override bool Equals(object obj)
        {
            OrderItemEntity item = obj as OrderItemEntity;
            if (item == null)
            {
                return false;
            }
            if (item.OrderId == OrderId && item.ProductId == ProductId)
            {
                return true;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return OrderId.GetHashCode() + ProductId.GetHashCode();
        }
    }
}
