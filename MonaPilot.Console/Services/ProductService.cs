using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MonaPilot.Console.Models;

namespace MonaPilot.Console.Services
{
    public class ProductService : IProductService
    {
        private readonly List<Product> _products;
        private readonly ILogger _logger;

        public ProductService(ILogger logger)
        {
            _logger = logger;
            // Ürün stoðu
            _products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop Deluxe", Type = "Electronics", Price = 1200, Stock = 5 },
                new Product { Id = 2, Name = "Smartphone Pro", Type = "Electronics", Price = 800, Stock = 10 },
                new Product { Id = 3, Name = "Tablet", Type = "Electronics", Price = 500, Stock = 8 },
                new Product { Id = 4, Name = "Office Chair", Type = "Furniture", Price = 300, Stock = 15 },
                new Product { Id = 5, Name = "Desk", Type = "Furniture", Price = 400, Stock = 7 },
                new Product { Id = 6, Name = "Book Bundle", Type = "Books", Price = 50, Stock = 100 }
            };
        }

        public async Task ProcessProductRequestAsync(ProductRequestEvent @event)
        {
            // Ürün tipine göre filtrele
            var suitableProducts = _products
                .Where(p => p.Type.Equals(@event.ProductType, StringComparison.OrdinalIgnoreCase) && p.Stock > 0 && p.Price <= @event.Budget)
                .OrderBy(p => p.Price)
                .ToList();

            if (!suitableProducts.Any())
            {
                _logger.LogWarning($"[UYARI] {@event.PersonName} için uygun ürün bulunamadý - Bütçe: {@event.Budget}, Tür: {@event.ProductType}");
                return;
            }

            // En uygun ürünü seç (bütçeye en yakýn)
            var selectedProduct = suitableProducts.First();
            selectedProduct.Stock--;

            _logger.LogInformation($"[SATIÞLAÞTIRMA] Kiþi: {@event.PersonName}, Ürün: {selectedProduct.Name}, Fiyat: {selectedProduct.Price}, Kalan Stok: {selectedProduct.Stock}");

            await Task.CompletedTask;
        }
    }
}
