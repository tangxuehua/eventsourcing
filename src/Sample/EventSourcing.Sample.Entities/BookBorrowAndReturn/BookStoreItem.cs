using System;

namespace EventSourcing.Sample.Entities
{
    public class BookStoreItemEntity
    {
        public Guid BookId { get; set; }
        public Guid LibraryId { get; set; }
        public int Count { get; set; }

        public override bool Equals(object obj)
        {
            BookStoreItemEntity item = obj as BookStoreItemEntity;
            if (item == null)
            {
                return false;
            }
            if (item.LibraryId == LibraryId && item.BookId == BookId)
            {
                return true;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return LibraryId.GetHashCode() + BookId.GetHashCode();
        }
    }
}
