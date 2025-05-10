using System.ComponentModel;
using ModelContextProtocol.Server;

namespace MCP_Server;

[McpServerToolType]
public sealed class FakeApiService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Fake-API");

    [McpServerTool]
    [Description("Creates a new product in the Fake Store API.")]
    public async Task<Product> CreateProductAsync(Product product)
    {
        var response = await _httpClient.PostAsJsonAsync("products", product);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Product>();
    }

    [McpServerTool]
    [Description("Retrieves a product by its ID from the Fake Store API.")]
    public async Task<Product> GetProductByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Product>($"products/{id}");
    }

    [McpServerTool]
    [Description("Retrieves all products from the Fake Store API.")]
    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Product>>("products");
    }

    [McpServerTool]
    [Description("Updates an existing product by its ID in the Fake Store API.")]
    public async Task<Product> UpdateProductAsync(int id, Product updatedProduct)
    {
        var response = await _httpClient.PutAsJsonAsync($"products/{id}", updatedProduct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Product>();
    }

    [McpServerTool]
    [Description("Deletes a product by its ID from the Fake Store API.")]
    public async Task<Product> DeleteProductAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"products/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Product>();
    }
}

public sealed class Product
{
    public int Id { get; set; } // Product ID
    public string Title { get; set; } = string.Empty; // Product title
    public double Price { get; set; } // Product price
    public string Description { get; set; } = string.Empty; // Product description
    public string Category { get; set; } = string.Empty; // Product category
    public string Image { get; set; } = string.Empty; // Product image URL
    public Rating Rating { get; set; } = new Rating(); // Product rating
}

public sealed class Rating
{
    public double Rate { get; set; } // Rating value
    public int Count { get; set; } // Number of ratings
}
