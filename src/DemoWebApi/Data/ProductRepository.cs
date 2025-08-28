using DemoWebApi.Models;
using LightTrace;

namespace DemoWebApi.Data;

public interface IProductRepository
{
    Task<Product> GetProductByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int id);
}

public class ProductRepository : IProductRepository
{
    // Simulated in-memory database
    private static readonly List<Product> Products = new()
    {
        new Product { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, Category = "Electronics", CreatedAt = DateTime.UtcNow.AddDays(-25) },
        new Product { Id = 2, Name = "Book", Description = "Programming guide", Price = 49.99m, Category = "Books", CreatedAt = DateTime.UtcNow.AddDays(-15) },
        new Product { Id = 3, Name = "Smartphone", Description = "Latest smartphone", Price = 799.99m, Category = "Electronics", CreatedAt = DateTime.UtcNow.AddDays(-5) },
        new Product { Id = 4, Name = "Coffee Mug", Description = "Ceramic coffee mug", Price = 12.99m, Category = "Home", CreatedAt = DateTime.UtcNow.AddDays(-2) }
    };

    public async Task<Product> GetProductByIdAsync(int id)
    {
        using var tracer = new Tracer($"ProductRepository.GetProductByIdAsync(id: {id})");
        
        // Simulate database query delay
        await Task.Delay(45);
        
        return Products.FirstOrDefault(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        using var tracer = new Tracer("ProductRepository.GetAllProductsAsync");
        
        // Simulate database query delay
        await Task.Delay(120);
        
        return Products.ToList();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        using var tracer = new Tracer($"ProductRepository.GetProductsByCategoryAsync(category: {category})");
        
        // Simulate database query delay with filtering
        await Task.Delay(80);
        
        return Products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        using var tracer = new Tracer($"ProductRepository.CreateProductAsync(name: {product.Name})");
        
        // Simulate database insert delay
        await Task.Delay(85);
        
        product.Id = Products.Max(p => p.Id) + 1;
        product.CreatedAt = DateTime.UtcNow;
        Products.Add(product);
        
        return product;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        using var tracer = new Tracer($"ProductRepository.UpdateProductAsync(id: {product.Id})");
        
        // Simulate database update delay
        await Task.Delay(70);
        
        var existingProduct = Products.FirstOrDefault(p => p.Id == product.Id);
        if (existingProduct != null)
        {
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Category = product.Category;
            return existingProduct;
        }
        
        return null;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        using var tracer = new Tracer($"ProductRepository.DeleteProductAsync(id: {id})");
        
        // Simulate database delete delay
        await Task.Delay(50);
        
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            Products.Remove(product);
            return true;
        }
        
        return false;
    }
}