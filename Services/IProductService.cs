using System.Collections.Generic;
using GlobalExceptionHandling.Models;

namespace GlobalExceptionHandling.Services
{
    public interface IProductService
    {
        Product? GetById(int id);
        IEnumerable<Product> GetAll();
    }
}