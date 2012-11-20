using System;
using System.Collections.Generic;

namespace EventSourcing.Sample.Entities
{
    public class LibraryAccountEntity
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Owner { get; set; }
        public IEnumerable<BorrowedBookEntity> BorrowedBooks { get; set; }
    }
}
