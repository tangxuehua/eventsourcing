using System;
using System.Collections.Generic;
using System.Linq;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.BookBorrowAndReturn
{
    public partial class LibraryAccount : AggregateRoot<Guid>
    {
        public LibraryAccount() { }
        public LibraryAccount(string number, string owner) : base(Guid.NewGuid())
        {
            OnEventHappened(new AccountCreated(Id, number, owner));
        }

        public string Number { get; private set; }
        public string Owner { get; private set; }
    }
    [SourcableEvent]
    public class AccountCreated
    {
        public Guid Id { get; private set; }
        public string Number { get; private set; }
        public string Owner { get; private set; }

        public AccountCreated(Guid id, string number, string owner)
        {
            Id = id;
            Number = number;
            Owner = owner;
        }
    }

    #region Borrower Role

    /// <summary>
    /// 借书人角色定义
    /// </summary>
    public interface IBorrower
    {
        /// <summary>
        /// 已接到的书本集合
        /// </summary>
        IEnumerable<BorrowedBook> BorrowedBooks { get; }
        /// <summary>
        /// 借书
        /// </summary>
        /// <param name="library"></param>
        /// <param name="book"></param>
        /// <param name="count"></param>
        void BorrowBook(Library library, Book book, int count);
        /// <summary>
        /// 还书
        /// </summary>
        /// <param name="library"></param>
        /// <param name="book"></param>
        /// <param name="count"></param>
        void ReturnBook(Library library, Book book, int count);
    }
    public partial class LibraryAccount : IBorrower
    {
        private IList<BorrowedBook> _borrowedBooks = new List<BorrowedBook>();

        IEnumerable<BorrowedBook> IBorrower.BorrowedBooks
        {
            get { return _borrowedBooks; }
        }
        void IBorrower.BorrowBook(Library library, Book book, int count)
        {
            library.LendBook(book, this, count);
            OnEventHappened(new BookBorrowed(library.Id, book.Id, Id, count));
        }
        void IBorrower.ReturnBook(Library library, Book book, int count)
        {
            library.ReceiveBook(book, this, count);
            OnEventHappened(new BookReturned(library.Id, book.Id, Id, count));
        }

        private void Handle(BookBorrowed evnt)
        {
            var borrowedBook = _borrowedBooks.SingleOrDefault(x => x.BookId == evnt.BookId);
            if (borrowedBook == null)
            {
                _borrowedBooks.Add(new BorrowedBook(Id, evnt.BookId, evnt.Count));
            }
            else
            {
                borrowedBook.SetCount(borrowedBook.Count + evnt.Count);
            }
        }
        private void Handle(BookReturned evnt)
        {
            var borrowedBook = _borrowedBooks.SingleOrDefault(x => x.BookId == evnt.BookId);
            borrowedBook.SetCount(borrowedBook.Count - evnt.Count);
            if (borrowedBook.Count == 0)
            {
                _borrowedBooks.Remove(borrowedBook);
            }
        }
    }
    public class BorrowedBook
    {
        public Guid AccountId { get; private set; }
        public Guid BookId { get; private set; }
        public int Count { get; private set; }

        protected BorrowedBook() { }
        public BorrowedBook(Guid accountId, Guid bookId, int count)
        {
            AccountId = accountId;
            BookId = bookId;
            Count = count;
        }

        internal void SetCount(int count)
        {
            Count = count;
        }

        public override bool Equals(object obj)
        {
            BorrowedBook item = obj as BorrowedBook;
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
    [SourcableEvent]
    public class BookBorrowed
    {
        public Guid LibraryId { get; private set; }
        public Guid BookId { get; private set; }
        public Guid AccountId { get; private set; }
        public int Count { get; private set; }

        public BookBorrowed(Guid libraryId, Guid bookId, Guid accountId, int count)
        {
            LibraryId = libraryId;
            BookId = bookId;
            AccountId = accountId;
            Count = count;
        }
    }
    [SourcableEvent]
    public class BookReturned
    {
        public Guid LibraryId { get; private set; }
        public Guid BookId { get; private set; }
        public Guid AccountId { get; private set; }
        public int Count { get; private set; }

        public BookReturned(Guid libraryId, Guid bookId, Guid accountId, int count)
        {
            LibraryId = libraryId;
            BookId = bookId;
            AccountId = accountId;
            Count = count;
        }
    }

    #endregion
}
