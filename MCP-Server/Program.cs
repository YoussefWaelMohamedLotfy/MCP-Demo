using MCP_Server;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMcpServer()
    //.WithStdioServerTransport()
    .WithHttpTransport()
    .WithToolsFromAssembly();

builder.Services.AddHttpClient<FakeApiService>("Fake-API", client =>
{
    client.BaseAddress = new Uri("https://fakestoreapi.com/");
    client.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddScoped<FakeApiService>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.MapMcp();

// API mappings
app.MapPost("/products", async (FakeApiService service, Product product) =>
{
    var result = await service.CreateProductAsync(product);
    return Results.Ok(result);
})
.WithName("CreateNewProduct")
.WithDescription("Creates a new product in the Fake Store API.");

app.MapGet("/products/{id:int}", async (FakeApiService service, int id) =>
{
    var result = await service.GetProductByIdAsync(id);
    return Results.Ok(result);
})
.WithName("GetProductById")
.WithDescription("Retrieves a product by its ID from the Fake Store API.");

app.MapGet("/products", async (FakeApiService service) =>
{
    var result = await service.GetAllProductsAsync();
    return Results.Ok(result);
})
.WithName("GetAllProducts")
.WithDescription("Retrieves all products from the Fake Store API.");

app.MapPut("/products/{id:int}", async (FakeApiService service, int id, Product updatedProduct) =>
{
    var result = await service.UpdateProductAsync(id, updatedProduct);
    return Results.Ok(result);
})
.WithName("UpdateProduct")
.WithDescription("Updates an existing product by its ID in the Fake Store API.");

app.MapDelete("/products/{id:int}", async (FakeApiService service, int id) =>
{
    var result = await service.DeleteProductAsync(id);
    return Results.Ok(result);
})
.WithName("DeleteProduct")
.WithDescription("Deletes a product by its ID from the Fake Store API.");

await app.RunAsync();
 