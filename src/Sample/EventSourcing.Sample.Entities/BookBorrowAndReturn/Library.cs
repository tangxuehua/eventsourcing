using System;
using System.Collections.Generic;

namespace EventSourcing.Sample.Entities
{
    public class LibraryEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<BookStoreItemEntity> BookStoreItems { get; set; }
    }
}
