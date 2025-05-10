var builder = DistributedApplication.CreateBuilder(args);

var mcpServer = builder.AddProject<Projects.MCP_Server>("mcp-server");

var agent = builder.AddProject<Projects.Agent_Server>("agent-server")
    .WithReference(mcpServer);

builder.AddProject<Projects.MCP_Client_Blazor>("mcp-client-blazor")
    .WithReference(agent);

await builder.Build().RunAsync();
