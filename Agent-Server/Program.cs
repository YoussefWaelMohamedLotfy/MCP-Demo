using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var mcpClient = await GetMCPClient();
var tools = await mcpClient.ListToolsAsync();

foreach (var tool in tools)
{
    Console.WriteLine($"{tool.Name} ({tool.Description})");
}

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddKernel()
    .AddOllamaChatCompletion("granite3.3", new Uri("http://localhost:11434"))
    .Plugins.AddFromFunctions("MCPSample", tools.Select(aiFunction => aiFunction.AsKernelFunction()));
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.MapGet("/chat", (string prompt, Kernel kernel, CancellationToken ct) =>
{
    // Enable automatic function calling
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    var executionSettings = new OllamaPromptExecutionSettings
    {
        Temperature = 0,
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
    };
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    //await kernel.ImportPluginFromOpenApiAsync("TestMcpServer",
    //    new Uri("https://localhost:7185/openapi/v1.json"),
    //    new()
    //    {
    //        EnablePayloadNamespacing = true
    //    },
    //    ct);
    return kernel.InvokePromptStreamingAsync<string>(prompt, new(executionSettings), cancellationToken: ct);
});

await app.RunAsync();

static async Task<IMcpClient> GetMCPClient()
{
    McpClientOptions options = new()
    {
        ClientInfo = new() { Name = "TestMcpServer", Version = "1.0.0" }
    };

    //var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
    //{
    //    Name = "TestMcpServer",
    //    Command = "dotnet",
    //    Arguments = ["run", "--project", "D:\\Projects\\My .NET Projects\\MCP-Demo\\MCP-Server\\MCP-Server.csproj"],
    //});

    var clientTransport = new SseClientTransport(new SseClientTransportOptions
    {
        Name = "TestMcpServer",
        Endpoint = new("https://localhost:7185/"),
        UseStreamableHttp = true,
        ConnectionTimeout = TimeSpan.FromMinutes(1)
    });

    var mcpClient = await McpClientFactory.CreateAsync(clientTransport, options);
    return mcpClient;
}
