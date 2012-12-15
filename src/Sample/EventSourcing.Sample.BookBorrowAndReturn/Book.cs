using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.BookBorrowAndReturn
{
    public class Book : AggregateRoot<Guid>
    {
        public Book() { }
        public Book(BookInfo bookInfo) : base(Guid.NewGuid())
        {
            OnEventHappened(new BookCreated(Id, bookInfo));
        }

        public BookInfo BookInfo { get; private set; }
    }
    public class BookInfo
    {
        public string Name { get; private set; }
        public string Author { get; private set; }
        public string Publisher { get; private set; }
        public string ISBN { get; private set; }
        public string Description { get; private set; }

        protected BookInfo() { }
        public BookInfo(string name, string isbn, string author, string publisher, string description)
        {
            Name = name;
            ISBN = isbn;
            Author = author;
            Publisher = publisher;
            Description = description;
        }
    }
    [SourcableEvent]
    public class BookCreated
    {
        public Guid Id { get; private set; }
        public BookInfo BookInfo { get; private set; }

        public BookCreated(Guid id, BookInfo bookInfo)
        {
            Id = id;
            BookInfo = bookInfo;
        }
    }
}
