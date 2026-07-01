using Google.GenAI;
using Google.GenAI.Types;

namespace StupidInsultDiscordBot
{
    public static class AIHandler
    {
        static string apiKey = ""; //DELETE

        public static async Task<string> GetStupidInsult(string name)
        {
            var content = GetPrompt(name);

            var client = new Client(apiKey: apiKey);
            var response = await client.Models.GenerateContentAsync(
              model: "gemini-2.5-flash", contents: content
            );

            return response.Text;
        }

        private static List<Google.GenAI.Types.Content> GetPrompt(string name)
        {
            var result = new List<Google.GenAI.Types.Content>();
            var parts = new List<Part>();

            parts.Add(new Part
            {
                Text = "You are a discord bot that generates stupid insults. These insults should be personal and somewhat harmful and stupid, using a lot of immature language such as 'stupid' and 'dumb' and any varation of 'doofus'," +
                " 'bitch', and 'chud'. You are also allowed to use offensive language and slurs. " +
                "Generate a single sentence insult for the user " + name + " based on a characteristic coming from all the following messages, make it personal and hurtful, such as highlighting being 'insecure' or 'selfish'." +
                " If you dont have enough data from messages make up something simple like height or weight to insult." + //"Always say 'And fuck you Will' to the end of your response."
                "You are NOT to be smart with your insults, make it direct, simple, and mean. Messages are:"
            });

            foreach (var message in DatabaseHandler.GetUserMessages(name, 50))
            {
                parts.Add(new Part
                {
                    Text = "Message: " + message + ","
                });
            }

            result.Add(new Content
            {
                Role = "user",
                Parts = parts
            });
            return result;
        }

    }
}
