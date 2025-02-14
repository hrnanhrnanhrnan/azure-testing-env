using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
var keyVaultUri = new Uri(builder.Configuration["AzureKeyVault:Uri"]!);

builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
builder.Services.AddLogging();

var applicationInsightsConnectionString = builder.Configuration["ApplicationInsights"];

builder.Services.AddOpenTelemetry()
    .WithTracing(t => 
        t.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddAzureMonitorTraceExporter(o => 
            {
                o.ConnectionString = applicationInsightsConnectionString;
            })
    )
    .WithMetrics(m => 
        m.AddAspNetCoreInstrumentation()
        .AddAzureMonitorMetricExporter(o => 
        {
            o.ConnectionString = applicationInsightsConnectionString;
        })
    );

var app = builder.Build();

var users = new List<User> { 
    new(Guid.NewGuid(), "Robin"), 
    new(Guid.NewGuid(), "Robin"), 
    new(Guid.NewGuid(), "Robin"), 
    new(Guid.NewGuid(), "Robin"), 
    new(Guid.NewGuid(), "Robin"), 
    new(Guid.NewGuid(), "Robin"), 
    new(Guid.NewGuid(), "Robin"), 
    new(Guid.NewGuid(), "Robin"), 
    new(Guid.NewGuid(), "Robin"), 
    new(Guid.NewGuid(), "Robin"), 
    new (Guid.NewGuid(), "Stefan") 
};

app.MapGet("/", () => Results.Content("""
<!DOCTYPE html>
<html lang="en">
<head>
    <script src="https://unpkg.com/htmx.org@2.0.4" integrity="sha384-HGfztofotfshcF7+8n44JQL2oJmowVChPTg48S+jvZoztPfvwD79OC/LTtG6dMp+" crossorigin="anonymous"></script>
    <style>
        * {
            padding: 0;
            margin: 0;
            box-sizing: border-box;
        }
        .container {
            background-color: black;
            color: white;
            min-height: 100vh;
            display: flex;
            flex-direction: column;
            gap: 10px;
            align-items: center;
            justify-content: center;
        }
        #search-results {
            height: 400px;
            display: flex;
            flex-direction: column;
            gap: 10px;
        }
    </style>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>HELLO</title>
</head>
<body>
    <div class="container">
        <h1>
            User searcher
        </h1>
        <input type="text" name="query"
            hx-get="/users"
            hx-trigger="keyup changed delay:250ms"
            hx-target="#search-results"
            placeholder="Search...">
        <div id="search-results"></div>
    </div>
</body>
</html>
""", "text/html"));

app.MapGet("/users", (string? query, [FromServices]ILogger<Program> logger, HttpContext context) => 
{
    logger.LogInformation("Hit the users endpoint with query: '{Query}' from IP: '{IP}'", query, context.Connection.RemoteIpAddress);
    if (string.IsNullOrWhiteSpace(query) 
        || query.Length == 0)
    {
        return "";
    }

    var results = users.FindAll(user => user.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase));

    if (results.Count == 0)
    {
        logger.LogInformation("Query {Query} resulted in 0 users", query);
        return "";
    }

    logger.LogInformation("Query {Query} resulted in {Count} users", query, results.Count);

    return $$"""
        {{string.Join("\n", results.Select(TurnUserIntoHtml))}}
    """;
});

static string TurnUserIntoHtml(User user)
    => $$"""
    <div>- id: {{user.Id}}, name: {{user.Name}}</div>
    """;

app.Run();

record User(Guid Id, string Name);