using Microsoft.EntityFrameworkCore;
using MonaPilot.API.Models;

namespace MonaPilot.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        public DbSet<BudgetRequest> BudgetRequests { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop Deluxe", Type = "Electronics", Price = 1200, Stock = 5 },
                new Product { Id = 2, Name = "Smartphone Pro", Type = "Electronics", Price = 800, Stock = 10 },
                new Product { Id = 3, Name = "Tablet", Type = "Electronics", Price = 500, Stock = 8 },
                new Product { Id = 4, Name = "Office Chair", Type = "Furniture", Price = 300, Stock = 15 },
                new Product { Id = 5, Name = "Desk", Type = "Furniture", Price = 400, Stock = 7 },
                new Product { Id = 6, Name = "Book Bundle", Type = "Books", Price = 50, Stock = 100 }
            );
        }
    }
}
