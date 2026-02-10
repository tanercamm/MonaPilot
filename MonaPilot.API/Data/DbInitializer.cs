using MonaPilot.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MonaPilot.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Veritabanýný oluþtur
            context.Database.EnsureCreated();

            // Ürünler zaten varsa devam et
            if (context.Products.Any())
            {
                return;
            }

            // Seed ürünler
            var products = new Product[]
            {
                new Product { Name = "Laptop Deluxe", Type = "Electronics", Price = 1200, Stock = 5 },
                new Product { Name = "Smartphone Pro", Type = "Electronics", Price = 800, Stock = 10 },
                new Product { Name = "Tablet", Type = "Electronics", Price = 500, Stock = 8 },
                new Product { Name = "Office Chair", Type = "Furniture", Price = 300, Stock = 15 },
                new Product { Name = "Desk", Type = "Furniture", Price = 400, Stock = 7 },
                new Product { Name = "Book Bundle", Type = "Books", Price = 50, Stock = 100 }
            };

            foreach (Product p in products)
            {
                context.Products.Add(p);
            }

            context.SaveChanges();
        }
    }
}
