using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EventSourcing.Sample.Model.BookBorrowAndReturn;
using EventSourcing.Sample.Model.Orders;

namespace EventSourcing.Sample.Model
{
    internal static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            Assert.IsNotNull(source);
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
    internal class Assert : NUnit.Framework.Assert
    {
        public static void IsNotNullOrWhiteSpace(string input)
        {
            Assert.IsNotNullOrEmpty(input);
            Assert.IsNotNullOrEmpty(input.Trim());
        }
        public static void IsValidKey(string input, int maxKeyLength = 255)
        {
            Assert.IsNotNullOrWhiteSpace(input);
            Assert.LessOrEqual(input.Length, maxKeyLength);
        }
        public static void IsValidEmail(string input)
        {
            var regex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            Assert.IsTrue(regex.IsMatch(input));
        }
        public static void IsValid(Product product)
        {
            Assert.IsNotNull(product);
            Assert.AreNotEqual(Guid.Empty, product.Id);
            Assert.IsValidKey(product.Name, 512);
            Assert.IsNotNullOrWhiteSpace(product.Description);
            Assert.Greater(product.Price, 0.0D);
        }
        public static void IsValid(Order order)
        {
            Assert.IsNotNull(order);
            Assert.AreNotEqual(Guid.Empty, order.Id);
            Assert.IsNotNullOrWhiteSpace(order.Customer);
            Assert.Greater(order.CreatedTime, new DateTime(1753, 1, 1));
        }
        public static void IsValid(OrderItem item)
        {
            Assert.IsNotNull(item);
            Assert.AreNotEqual(Guid.Empty, item.OrderId);
            Assert.AreNotEqual(Guid.Empty, item.ProductId);
            Assert.Greater(item.Price, 0.0D);
            Assert.Greater(item.Amount, 0.0D);
        }
        public static void IsValid(Library library)
        {
            Assert.IsNotNull(library);
            Assert.AreNotEqual(Guid.Empty, library.Id);
            Assert.IsValidKey(library.Name, 512);
        }
        public static void IsValid(BookInfo bookInfo)
        {
            Assert.IsNotNull(bookInfo);
            Assert.IsValidKey(bookInfo.Name, 512);
            Assert.IsValidKey(bookInfo.ISBN, 256);
            Assert.IsValidKey(bookInfo.Author, 512);
            Assert.IsValidKey(bookInfo.Publisher, 512);
        }
        public static void IsValid(Book book)
        {
            Assert.IsNotNull(book);
            Assert.AreNotEqual(Guid.Empty, book.Id);
            Assert.IsValid(book.BookInfo);
        }
        public static void IsValid(LibraryAccount account)
        {
            Assert.IsNotNull(account);
            Assert.AreNotEqual(Guid.Empty, account.Id);
            Assert.IsValidKey(account.Number, 128);
            Assert.IsValidKey(account.Owner, 512);
        }
    }
}
