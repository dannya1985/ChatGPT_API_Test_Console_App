using Microsoft.Extensions.Configuration;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using System.Net;
using System;
using System.Diagnostics;
using System.IO;

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
    Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
}


var imageResult = await openAiService.Image.CreateImage(new ImageCreateRequest
{
    Prompt = "Flying super dog against a blue sky with clouds and sun",
    N = 4,
    Size = StaticValues.ImageStatics.Size.Size1024,
    ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
    User = "TestUser"
});


if (imageResult.Successful)
{
    using (WebClient client = new WebClient())
    {
        int i = 0;
        foreach(var u in imageResult.Results)
        {
            //cheesy way of making the filenames differ
            var filename = @"image-" + DateTime.Now.ToShortDateString().Replace('/', '-') + "-" + DateTime.Now.ToLongTimeString().Replace(':','-') + ".png";

            //download the output image to the working directory
            client.DownloadFile(new Uri(u.Url), filename);
            
            //ghetto way of making sure the file name is different due to using timestamps
            System.Threading.Thread.Sleep(1000);
        }        
    }

    //display the file urls
    Console.WriteLine(string.Join("\n\n", imageResult.Results.Select(r => r.Url)));

    //open explorer to see the output files
    Process.Start("explorer.exe", "/open, " + new Uri(
    System.IO.Path.GetDirectoryName(
        System.Reflection.Assembly.GetExecutingAssembly().CodeBase)
    ).LocalPath);
}