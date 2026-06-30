using Newtonsoft.Json.Linq;
using System.Drawing;
using Discord;
using Discord.WebSocket;


namespace StupidInsultDiscordBot
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        public Bot()
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds
                                | GatewayIntents.GuildMessages
                                | GatewayIntents.MessageContent  // required to read text
            };

            _client = new DiscordSocketClient(config);
            _client.Log += OnLog;
            _client.MessageReceived += OnMessageReceived;
        }

        private Task OnLog(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public async Task StartAsync(string token)
        {
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }

        private async Task OnMessageReceived(SocketMessage message)
        {
            if (message.Author.IsBot) return;

            DatabaseHandler.LogMessage(message.Author.Username, message.Content);

            // Command: !computahinsult @someone
            if (message.Content.StartsWith("!computahinsult"))
            {
                await HandleComputahInsultCommand(message);
            }
        }

        private async Task HandleComputahInsultCommand(SocketMessage message)
        {
            // Get the mentioned user, or default to the message author
            var target = message.MentionedUsers.FirstOrDefault() as SocketUser
                         ?? message.Author;

            if (target.Id == message.Author.Id && message.MentionedUsers.Count == 0)
            {
                await message.Channel.SendMessageAsync(
                    "Tag someone, e.g. `!computahinsult @friend`");
                return;
            }

            string aiResponse = await AIHandler.GetStupidInsult(target.Username);

            var embed = new EmbedBuilder()
                .WithDescription(aiResponse)
                .Build();

            await message.Channel.SendMessageAsync(embed: embed);
        }
    }
}
