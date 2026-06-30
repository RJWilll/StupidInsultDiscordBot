using StupidInsultDiscordBot;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

DatabaseHandler.Initialize();
Bot bot = new Bot();
bot.StartAsync(""); //DELETE DISCORD KEY

await Task.Delay(-1); // keep the bot running forever

app.Run();
