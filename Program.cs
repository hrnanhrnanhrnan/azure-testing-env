var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var users = new List<User> { new(Guid.NewGuid(), "Robin"), new (Guid.NewGuid(), "Stefan") };

app.MapGet("/", () => "Hello World!");
app.MapGet("/users", () => users);

app.Run();


record User(Guid Id, string Name);