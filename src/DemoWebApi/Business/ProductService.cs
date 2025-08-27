using DemoWebApi.Data;
using DemoWebApi.Models;
using LightTrace;

namespace DemoWebApi.Business;

public interface IProductService
{
    Task<Product> GetProductAsync(int id);
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int id);
    Task<decimal> CalculateDiscountedPriceAsync(int productId, decimal discountPercentage);
    Task<IEnumerable<string>> GetCategoriesAsync();
}

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product> GetProductAsync(int id)
    {
        using var tracer = new Tracer($"ProductService.GetProductAsync(id: {id})");
        
        if (id <= 0)
            throw new ArgumentException("Product ID must be greater than 0", nameof(id));

        var product = await _productRepository.GetProductByIdAsync(id);
        
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found");

        return product;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        using var tracer = new Tracer("ProductService.GetAllProductsAsync");
        
        var products = await _productRepository.GetAllProductsAsync();
        
        // Simulate some business logic processing
        await Task.Delay(15);
        
        return products.OrderBy(p => p.Category).ThenBy(p => p.Name);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        using var tracer = new Tracer($"ProductService.GetProductsByCategoryAsync(category: {category})");
        
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be empty", nameof(category));

        var products = await _productRepository.GetProductsByCategoryAsync(category);
        
        // Simulate some business logic processing
        await Task.Delay(10);
        
        return products.OrderBy(p => p.Price);
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        using var tracer = new Tracer($"ProductService.CreateProductAsync(name: {product?.Name})");
        
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Product name is required", nameof(product));

        if (string.IsNullOrWhiteSpace(product.Category))
            throw new ArgumentException("Product category is required", nameof(product));

        if (product.Price <= 0)
            throw new ArgumentException("Product price must be greater than 0", nameof(product));

        // Simulate business validation
        await ValidateProductBusinessRulesAsync(product);

        return await _productRepository.CreateProductAsync(product);
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        using var tracer = new Tracer($"ProductService.UpdateProductAsync(id: {product?.Id})");
        
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        if (product.Id <= 0)
            throw new ArgumentException("Product ID must be greater than 0", nameof(product));

        // Check if product exists
        var existingProduct = await _productRepository.GetProductByIdAsync(product.Id);
        if (existingProduct == null)
            throw new KeyNotFoundException($"Product with ID {product.Id} not found");

        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Product name is required", nameof(product));

        if (string.IsNullOrWhiteSpace(product.Category))
            throw new ArgumentException("Product category is required", nameof(product));

        if (product.Price <= 0)
            throw new ArgumentException("Product price must be greater than 0", nameof(product));

        // Simulate business validation
        await ValidateProductBusinessRulesAsync(product);

        return await _productRepository.UpdateProductAsync(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        using var tracer = new Tracer($"ProductService.DeleteProductAsync(id: {id})");
        
        if (id <= 0)
            throw new ArgumentException("Product ID must be greater than 0", nameof(id));

        // Check if product exists
        var existingProduct = await _productRepository.GetProductByIdAsync(id);
        if (existingProduct == null)
            throw new KeyNotFoundException($"Product with ID {id} not found");

        return await _productRepository.DeleteProductAsync(id);
    }

    public async Task<decimal> CalculateDiscountedPriceAsync(int productId, decimal discountPercentage)
    {
        using var tracer = new Tracer($"ProductService.CalculateDiscountedPriceAsync(id: {productId}, discount: {discountPercentage}%)");
        
        if (discountPercentage < 0 || discountPercentage > 100)
            throw new ArgumentException("Discount percentage must be between 0 and 100", nameof(discountPercentage));

        var product = await GetProductAsync(productId);
        
        // Simulate complex discount calculation logic
        await Task.Delay(25);
        
        var discountAmount = product.Price * (discountPercentage / 100);
        return product.Price - discountAmount;
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
    {
        using var tracer = new Tracer("ProductService.GetCategoriesAsync");
        
        var products = await _productRepository.GetAllProductsAsync();
        
        // Simulate processing
        await Task.Delay(5);
        
        return products.Select(p => p.Category).Distinct().OrderBy(c => c);
    }

    private async Task ValidateProductBusinessRulesAsync(Product product)
    {
        using var tracer = new Tracer($"ProductService.ValidateProductBusinessRulesAsync(name: {product.Name})");
        
        // Simulate complex business validation
        await Task.Delay(30);
        
        // Example business rule: Electronics must have a price above $10
        if (product.Category.Equals("Electronics", StringComparison.OrdinalIgnoreCase) && product.Price < 10)
            throw new ArgumentException("Electronics products must have a price of at least $10");
        
        // Example business rule: Books cannot exceed $200
        if (product.Category.Equals("Books", StringComparison.OrdinalIgnoreCase) && product.Price > 200)
            throw new ArgumentException("Books cannot have a price exceeding $200");
    }
}