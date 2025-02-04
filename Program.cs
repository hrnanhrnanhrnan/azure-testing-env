var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var users = new List<User> { new(Guid.NewGuid(), "Robin"), new (Guid.NewGuid(), "Stefan") };

app.MapGet("/", () => "Hello updated world!");
app.MapGet("/users", (string? query) => query is not null ? users.Where(x => x.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase)) : users);

app.Run();

record User(Guid Id, string Name);