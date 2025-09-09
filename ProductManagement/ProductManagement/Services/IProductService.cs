using ProductManagement.Models;

namespace ProductManagement.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAllProducts();
        Product? GetProductById(int id);
    }
}