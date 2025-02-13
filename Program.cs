var builder = WebApplication.CreateBuilder(args);
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
            hx-trigger="keyup changed delay:500ms"
            hx-target="#search-results"
            placeholder="Search...">
        <div id="search-results"></div>
    </div>
</body>
</html>
""", "text/html"));

app.MapGet("/users", (string? query) => 
{
    if (string.IsNullOrWhiteSpace(query) 
        || query.Length == 0)
    {
        return "";
    }

    var results = users.FindAll(user => user.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase));

    return results.Count == 0
        ? ""
        : $$"""
            {{string.Join("\n", results.Select(TurnUserIntoHtml))}}
        """;
});

static string TurnUserIntoHtml(User user)
    => $$"""
    <div>- id: {{user.Id}}, name: {{user.Name}}</div>
    """;

app.Run();

record User(Guid Id, string Name);