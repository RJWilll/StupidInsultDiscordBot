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

            if(DatabaseHandler.IsUserBlocked(message.Author.Username))
            {
                // Don't log messages from blocked users
                return;
            }

            DatabaseHandler.LogMessage(message.Author.Username, message.Content);

            // Command: !computahinsult @someone
            if (message.Content.StartsWith("!computahinsult"))
            {
                await HandleComputahInsultCommand(message);
            }
            else if(message.Content.StartsWith("!computahforget"))
            {
                await HandleComputahForgetCommand(message);
            }
            else if(message.Content.StartsWith("!computahblock"))
            {
                await HandleComputahBlockCommand(message);
            }
            else if(message.Content.StartsWith("!computahunblock"))
            {
                await HandleComputahUnblockCommand(message);
            }
        }

        private async Task HandleComputahUnblockCommand(SocketMessage message)
        {
            var target = message.MentionedUsers.FirstOrDefault() as SocketUser
                         ?? message.Author;

            if (target.Id == message.Author.Id && message.MentionedUsers.Count == 0)
            {
                await message.Channel.SendMessageAsync(
                    "Tag someone, e.g. `!computahunblock @friend`");
                return;
            }

            DatabaseHandler.unblockUser(target.Username);
            await message.Channel.SendMessageAsync(
                $"Unblocked {target.Username}. They can now be insulted again.");
        }

        private async Task HandleComputahBlockCommand(SocketMessage message)
        {
            var target = message.MentionedUsers.FirstOrDefault() as SocketUser
                         ?? message.Author;

            if (target.Id == message.Author.Id && message.MentionedUsers.Count == 0)
            {
                await message.Channel.SendMessageAsync(
                    "Tag someone, e.g. `!computahblock @friend`");
                return;
            }

            DatabaseHandler.ForgetUserMessages(target.Username);
            DatabaseHandler.BlockUser(target.Username);
            await message.Channel.SendMessageAsync(
                $"Blocked {target.Username}. They will no longer be able to be insulted and recent messages are deleted. No more messages will be stored for insults.");
        }

        private async Task HandleComputahInsultCommand(SocketMessage message)
        {
            var target = message.MentionedUsers.FirstOrDefault() as SocketUser
                         ?? message.Author;

            if (target.Id == message.Author.Id && message.MentionedUsers.Count == 0)
            {
                await message.Channel.SendMessageAsync(
                    "Tag someone, e.g. `!computahinsult @friend`");
                return;
            }

            if (DatabaseHandler.IsUserBlocked(target.Username))
            {
                await message.Channel.SendMessageAsync(
                    $"{target.Username} is blocked and cannot be insulted.");
                return;
            }

            string aiResponse = await AIHandler.GetStupidInsult(target.Username);

            var embed = new EmbedBuilder()
                .WithDescription(aiResponse)
                .Build();

            await message.Channel.SendMessageAsync(embed: embed);
        }

        private async Task HandleComputahForgetCommand(SocketMessage message)
        {
            // Get the mentioned user, or default to the message author
            var target = message.MentionedUsers.FirstOrDefault() as SocketUser
                         ?? message.Author;
            if (target.Id == message.Author.Id && message.MentionedUsers.Count == 0)
            {
                await message.Channel.SendMessageAsync(
                    "Tag someone, e.g. `!computahforget @friend`");
                return;
            }
            DatabaseHandler.ForgetUserMessages(target.Username);
            await message.Channel.SendMessageAsync(
                $"Forgot all messages for {target.Username}. I love your privacy :)");
        }
    }
}
