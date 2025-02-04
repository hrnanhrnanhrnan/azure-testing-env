var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var users = new List<User> { new(Guid.NewGuid(), "Robin"), new (Guid.NewGuid(), "Stefan") };

app.MapGet("/", () => Results.Content("""
<!DOCTYPE html>
<html lang="en">
<head>
    <style>
        * {
            padding: 0;
            margin: 0;
            box-sizing: border-box;
        }
        .container {
            background-color: black;
            color: white;
            height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }
    </style>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>HELLO</title>
</head>
<body>
    <div class="container">
        <h1>
            Hello world
        </h1>
    </div>
</body>
</html>
""", "text/html"));
app.MapGet("/users", (string? query) => query is not null ? users.Where(x => x.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase)) : users);

app.Run();

record User(Guid Id, string Name);