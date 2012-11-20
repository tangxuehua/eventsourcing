using System;

namespace EventSourcing.Sample.Entities
{
    public class BorrowedBookEntity
    {
        public Guid AccountId { get; set; }
        public Guid BookId { get; set; }
        public int Count { get; set; }

        public override bool Equals(object obj)
        {
            BorrowedBookEntity item = obj as BorrowedBookEntity;
            if (item == null)
            {
                return false;
            }
            if (item.AccountId == AccountId && item.BookId == BookId)
            {
                return true;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return AccountId.GetHashCode() + BookId.GetHashCode();
        }
    }
}
