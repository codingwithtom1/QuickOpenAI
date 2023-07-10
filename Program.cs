using Azure.AI.OpenAI;
using Azure;

namespace QuickOpenAi
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            Console.WriteLine("OpenAI Test");
            await NonStreamingChat();
            //await StreamingChat();
        }

        static async Task StreamingChat()
        {
            OpenAIClient client = new OpenAIClient(
                new Uri(uri),
                new AzureKeyCredential(APIKEY));

            ChatCompletionsOptions options = new ChatCompletionsOptions()
            {
                Messages = { new ChatMessage(ChatRole.System, @"You are an AI assistant that helps people find information.") },
                Temperature = (float)0.7,
                MaxTokens = 800,
                NucleusSamplingFactor = (float)0.95,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            };

            while (true)
            {
                Console.Write("Chat Prompt:");
                string line = Console.ReadLine()!;
                if (line.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                options.Messages.Add(new ChatMessage(ChatRole.User, line));

                Console.WriteLine("Response:");
                Response<StreamingChatCompletions> response =
                await client.GetChatCompletionsStreamingAsync(
                    deploymentOrModelName: deployment,
                    options);

                using StreamingChatCompletions streamingChatCompletions = response.Value;
                string fullresponse = "";
                await foreach (StreamingChatChoice choice in streamingChatCompletions.GetChoicesStreaming())
                {
                    await foreach (ChatMessage message in choice.GetMessageStreaming())
                    {
                        fullresponse += message.Content;
                        Console.Write(message.Content);
                    }
                    Console.WriteLine();
                }
                options.Messages.Add(new ChatMessage(ChatRole.Assistant, fullresponse));

            }
        }

        static async Task NonStreamingChat()
        {
            OpenAIClient client = new OpenAIClient(
                new Uri(uri),
                new AzureKeyCredential(APIKEY));

            ChatCompletionsOptions options = new ChatCompletionsOptions()
            {
                Messages = { new ChatMessage(ChatRole.System, @"You are an AI assistant that helps people find information.") },
                Temperature = (float)0.7,
                MaxTokens = 800,
                NucleusSamplingFactor = (float)0.95,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            };

            while (true)
            {
                Console.Write("Chat Prompt:");
                string line = Console.ReadLine()!;
                if (line.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                options.Messages.Add(new ChatMessage(ChatRole.User, line));

                Console.WriteLine("Response:");
                Response<ChatCompletions> response =
                await client.GetChatCompletionsAsync(
                    deploymentOrModelName: deployment,
                    options);

                ChatCompletions completions  = response.Value;
                string fullresponse = completions.Choices[0].Message.Content;
                Console.WriteLine(fullresponse);
                options.Messages.Add(completions.Choices[0].Message);

            }
        }









        public static readonly string APIKEY = "YOURKEY";
        public static readonly string deployment = "YOURDEPLOYMENT";
        public static readonly string uri = "https://YOURURI.openai.azure.com/";

    }
}