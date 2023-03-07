using Microsoft.Extensions.Configuration;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;

Console.WriteLine("Hello, ChatGPT world!");

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var secretProvider = config.Providers.First();
secretProvider.TryGet("ChatGPT:ServiceApiKey", out var secretPass);

var openAiService = new OpenAIService(new OpenAiOptions()
{
    ApiKey = secretPass
});



var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
{
    Messages = new List<ChatMessage>
    {
        ChatMessage.FromUser("Who won the last FIFA world cup?")
    },
    Model = Models.ChatGpt3_5Turbo,
    MaxTokens = 50//optional
});

if (completionResult.Successful)
{
    Console.WriteLine(completionResult.Choices.First().Message.Content);
}
else
{
    Console.WriteLine(completionResult.Error.ToString());
}