using System;
using System.Collections.Generic;
using System.Linq;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.BookBorrowAndReturn
{
    public class Library : AggregateRoot<Guid>
    {
        private IList<BookStoreItem> _bookStoreItems = new List<BookStoreItem>();

        public Library() { }
        public Library(string name) : base(Guid.NewGuid())
        {
            OnEventHappened(new LibraryCreated(Id, name));
        }

        public string Name { get; private set; }
        public IEnumerable<BookStoreItem> BookStoreItems { get { return _bookStoreItems; } }

        /// <summary>
        /// 图书入库
        /// </summary>
        /// <param name="book"></param>
        /// <param name="count"></param>
        public void StoreBook(Book book, int count)
        {
            var bookStoreItem = _bookStoreItems.SingleOrDefault(x => x.BookId == book.Id);
            if (bookStoreItem == null)
            {
                OnEventHappened(new NewBookStored(Id, book.Id, count));
            }
            else
            {
                OnEventHappened(new BookCountUpdated(Id, book.Id, bookStoreItem.Count + count));
            }
        }
        /// <summary>
        /// 图书馆借出书
        /// </summary>
        /// <param name="book"></param>
        /// <param name="account"></param>
        /// <param name="count"></param>
        public void LendBook(Book book, LibraryAccount account, int count)
        {
            var bookStoreItem = _bookStoreItems.SingleOrDefault(x => x.BookId == book.Id);
            OnEventHappened(new BookLent(Id, book.Id, account.Id, count));
            OnAggregateRootCreated(new HandlingEvent(book, account, this, HandlingType.Borrow));
        }
        /// <summary>
        /// 图书馆接收归还的书
        /// </summary>
        /// <param name="book"></param>
        /// <param name="account"></param>
        /// <param name="count"></param>
        public void ReceiveBook(Book book, LibraryAccount account, int count)
        {
            var bookStoreItem = _bookStoreItems.SingleOrDefault(x => x.BookId == book.Id);
            OnEventHappened(new BookReceived(Id, book.Id, account.Id, count));
            OnAggregateRootCreated(new HandlingEvent(book, account, this, HandlingType.Return));
        }
        /// <summary>
        /// 获取图书馆中某本书的库存信息
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public BookStoreItem GetBookStoreItem(Guid bookId)
        {
            return _bookStoreItems.SingleOrDefault(x => x.BookId == bookId);
        }

        private void Handle(NewBookStored evnt)
        {
            _bookStoreItems.Add(new BookStoreItem(evnt.BookId, Id, evnt.Count));
        }
        private void Handle(BookCountUpdated evnt)
        {
            _bookStoreItems.Single(x => x.BookId == evnt.BookId).SetCount(evnt.Count);
        }
        private void Handle(BookLent evnt)
        {
            var bookStoreItem = _bookStoreItems.Single(x => x.BookId == evnt.BookId);
            bookStoreItem.SetCount(bookStoreItem.Count - evnt.Count);
        }
        private void Handle(BookReceived evnt)
        {
            var bookStoreItem = _bookStoreItems.Single(x => x.BookId == evnt.BookId);
            bookStoreItem.SetCount(bookStoreItem.Count + evnt.Count);
        }
    }
    public class BookStoreItem
    {
        public Guid BookId { get; private set; }
        public Guid LibraryId { get; private set; }
        public int Count { get; private set; }

        protected BookStoreItem() { }
        public BookStoreItem(Guid bookId, Guid libraryId, int count)
        {
            BookId = bookId;
            LibraryId = libraryId;
            Count = count;
        }

        internal void SetCount(int count)
        {
            Count = count;
        }

        public override bool Equals(object obj)
        {
            BookStoreItem item = obj as BookStoreItem;
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
    [SourcableEvent]
    public class LibraryCreated
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public LibraryCreated(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
    [SourcableEvent]
    public class NewBookStored
    {
        public Guid LibraryId { get; private set; }
        public Guid BookId { get; private set; }
        public int Count { get; private set; }

        public NewBookStored(Guid libraryId, Guid bookId, int count)
        {
            LibraryId = libraryId;
            BookId = bookId;
            Count = count;
        }
    }
    [SourcableEvent]
    public class BookCountUpdated
    {
        public Guid LibraryId { get; private set; }
        public Guid BookId { get; private set; }
        public int Count { get; private set; }

        public BookCountUpdated(Guid libraryId, Guid bookId, int count)
        {
            LibraryId = libraryId;
            BookId = bookId;
            Count = count;
        }
    }
    [SourcableEvent]
    public class BookLent
    {
        public Guid LibraryId { get; private set; }
        public Guid BookId { get; private set; }
        public Guid AccountId { get; private set; }
        public int Count { get; private set; }

        public BookLent(Guid libraryId, Guid bookId, Guid accountId, int count)
        {
            LibraryId = libraryId;
            BookId = bookId;
            AccountId = accountId;
            Count = count;
        }
    }
    [SourcableEvent]
    public class BookReceived
    {
        public Guid LibraryId { get; private set; }
        public Guid BookId { get; private set; }
        public Guid AccountId { get; private set; }
        public int Count { get; private set; }

        public BookReceived(Guid libraryId, Guid bookId, Guid accountId, int count)
        {
            LibraryId = libraryId;
            BookId = bookId;
            AccountId = accountId;
            Count = count;
        }
    }
}
