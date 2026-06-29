var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

await Task.Delay(-1); // keep the bot running forever

app.Run();
