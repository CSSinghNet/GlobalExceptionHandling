using GlobalExceptionHandling.Api.Exceptions;
using GlobalExceptionHandling.Models;
using System.Collections.Generic;
using System.Linq;

namespace GlobalExceptionHandling.Services
{
    public class DummyProductService : IProductService
    {
        private readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Red T-Shirt", Description = "Comfortable cotton t-shirt", Price = 19.99m },
            new Product { Id = 2, Name = "Blue Jeans", Description = "Classic denim jeans", Price = 49.99m },
            new Product { Id = 3, Name = "Coffee Mug", Description = "Ceramic mug, 350ml", Price = 9.99m }
        };

        //public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);

        public Product GetById(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                throw new NotFoundException("Product", id);
            }
            return product;
        }


        public IEnumerable<Product> GetAll() => _products;
    }
}