using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookStoreSqlLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new DbContextOptionsBuilder<BookStoreContext>();
            builder.UseSqlServer("Server=tcp:concept-proofing-sqlserver.database.windows.net,1433;Initial Catalog=concept-proofing-sql;Persist Security Info=False;User ID=cory;Password=qucgA7eNBiMn;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            //builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=concept-proofing-sql;Trusted_Connection=true;MultipleActiveResultSets=true");
           
            var context = new BookStoreContext(builder.Options);
            var random = new Random();
            LoadAuthors(context, 1250000);
            Console.WriteLine("Finished loading authors");
            LoadPublishers(context, 125000);
            Console.WriteLine("Finished loading publishers");
            LoadBooks(context, random, 1250000, 125000, 1250000);
            Console.WriteLine("Finished loading books");
            LoadStoreInventory(context, random, 1250000, 1250000);
            Console.WriteLine("Finished loading inventory");
            LoadCustomers(context, random, 1000);
            Console.WriteLine("Finished loading customers");
            LoadOrders(context, random, 1000, 1250000);
            Console.WriteLine("Finished loading orders");
            LoadOrderedItems(context, random, 1250000, 1250000, 1250000);
            Console.WriteLine("Finished loading orderedItems");

            //LoadAuthors(context, 125);
            //LoadPublishers(context, 125);
            //LoadBooks(context, random, 125, 125, 1250);
            //LoadStoreInventory(context, random, 1250, 1250);
            //LoadCustomers(context, random, 200);
            //LoadOrders(context, random, 200, 1250);
            //LoadOrderedItems(context, random, 1250, 1250, 1250);

            context.Dispose();

            Console.WriteLine("Finished loading database");
        }

        private static void LoadAuthors(BookStoreContext context, int requestedCreatedAuthors)
        {
            for (int i = 1; i <= requestedCreatedAuthors; i++)
            {
                var newAuthor = new Author { Name = $"Author - {i}", Country = $"Country - {i}" };
                context.Add(newAuthor);
            }
            context.SaveChanges();
        }

        private static void LoadPublishers(BookStoreContext context, int requestedPublishers)
        {
            for (int i = 1; i <= requestedPublishers; i++)
            {
                var newPublisher = new Publisher { Name = $"Publisher - {i}", Country = $"Country - {i}",  };
                context.Add(newPublisher);
            }
            context.SaveChanges();
        }

        private static void LoadBooks(BookStoreContext context, Random random, int createdAuthor, int createdPublisher, int requestedCreatedBooks)
        {
            for (int i = 1; i <= requestedCreatedBooks; i++)
            {
                var newBook = new Book { 
                    Price = random.Next(50, 250),
                    Edition = random.Next(1, 10), 
                    AuthorId = random.Next(1, createdAuthor),
                    PublisherId = random.Next(1, createdPublisher)
                };
                context.Add(newBook);
            }
            context.SaveChanges();
        }

        private static void LoadStoreInventory(BookStoreContext context, Random random, int createdBooks, int requestedInventory)
        {
            for (int i = 1; i <= requestedInventory; i++)
            {
                var newInventory = new Inventory
                {
                    StockLevelUsed = random.Next(1, 10),
                    StockLevelNew = random.Next(10, 50),
                    BookId = random.Next(1, createdBooks)
                };
                context.Add(newInventory);
            }
            context.SaveChanges();
        }

        private static void LoadCustomers(BookStoreContext context, Random random, int requestedCustomers)
        {
            for (int i = 1; i <= requestedCustomers; i++)
            {
                var newCustomer = new Customer {
                    Name = $"customer - {i}",
                    Address = $"{random.Next(1, 10000)}, easy street",
                    PostalCode = random.Next(10000, 99999).ToString(),
                    State = $"State - {i}",
                    Country = $"Country - {i}",
                    PhoneNumber = GeneratePhoneNumber(random),
                };
                context.Add(newCustomer);
            }

            context.SaveChanges();
        }

        private static void LoadOrders(BookStoreContext context, Random random, int createdCustomers, int requestedOrders)
        {
            for (int i = 1; i <= requestedOrders; i++)
            {
                var newOrder = new Order
                {
                    OrderDate = DateTime.UtcNow,
                    OrderTotal = new decimal(random.NextDouble() * (10000.99 - 10.00) + 10.00),
                    CustomerId = random.Next(1, createdCustomers),
                };
                context.Add(newOrder);
            }

            context.SaveChanges();
        }

        private static void LoadOrderedItems(BookStoreContext context, Random random, int createdBooks, int createdOrders, int requestedOrderedItems)
        {
            var newOrderedItems = new List<OrderedItem>();

            for (int i = 1; i <= requestedOrderedItems; i++)
            {
                var newOrderedItem = new OrderedItem
                {
                    Quantity = random.Next(0, 100),
                    Price = new decimal(random.NextDouble() * (10000.99 - 10.00) + 10.00),
                    OrderId = random.Next(1, createdOrders),
                    BookId = random.Next(1, createdBooks),
                };
                newOrderedItems.Add(newOrderedItem);
            }

            var distinctOrderedItems = newOrderedItems.DistinctBy(_ => new { _.BookId, _.OrderId });

            context.AddRange(distinctOrderedItems);
            context.SaveChanges();
        }

        private static string GeneratePhoneNumber(Random random)
        {
            string s = string.Empty;
            for (int i = 1; i < 11; i++)
                s = String.Concat(s, random.Next(9).ToString());
            return s;
        }

    }
}
