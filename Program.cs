using Microsoft.Extensions.Configuration;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using System.Net;
using System.Diagnostics;
using System.Reflection;
using OpenAI.GPT3.ObjectModels.SharedModels;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.ObjectModels.ResponseModels.ImageResponseModel;

namespace ChatGPT_API_Test_Console_App
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            bool exit = false;

            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

            var secretProvider = config.Providers.First();
            secretProvider.TryGet("ChatGPT:ServiceApiKey", out var secretPass);

            var openAiService = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = secretPass
            });

            while (!exit)
            {
                switch (Program.ShowMenu())
                {
                    case 0: 
                        Console.WriteLine("");
                        exit = true;
                        break;
                    case 1: 
                        Console.WriteLine("");
                        await PromptChatGpt(openAiService);
                        break;
                    case 2: 
                        Console.WriteLine("");
                        await PromptDalle(openAiService);
                        break;
                }
            }
        }

        private static async Task PromptDalle(IOpenAIService service, string prompt = "Flying super dog against a blue sky with clouds and sun", int imageCount = 2)
        {
            Console.WriteLine("Current prompt: " + prompt);
            Console.WriteLine("Enter prompt: ");

            var input = Console.ReadLine().ToLower();
            if (!string.IsNullOrEmpty(input))
            {
                prompt = input;
            }

            Console.WriteLine("Current Image generation count: " + imageCount);
            Console.WriteLine("Enter to continue, or type the desired amount of images (max is 10): ");

            input = Console.ReadLine().ToLower();
            if (!string.IsNullOrEmpty(input))
            {
                imageCount = int.Parse(input);
                if (imageCount > 10)
                {
                    imageCount = 10;
                }
                else if (imageCount < 1)
                {
                    imageCount = 1;
                }
            }
            else
            {
                imageCount = 2;
            }

            var imageResult = await service.Image.CreateImage(new ImageCreateRequest
            {
                Prompt = prompt,
                N = imageCount,
                Size = StaticValues.ImageStatics.Size.Size1024,
                ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
                User = "TestUser"
            });


            if (imageResult.Successful)
            {
                using (WebClient client = new WebClient())
                {
                    int i = 0;
                    foreach (var u in imageResult.Results)
                    {
                        //cheesy way of making the filenames differ
                        var filename = @"image-" + DateTime.Now.ToShortDateString().Replace('/', '-') + "-" + DateTime.Now.ToLongTimeString().Replace(':', '-') + ".png";

                        //download the output image to the working directory
                        client.DownloadFile(new Uri(u.Url), filename);

                        //ghetto way of making sure the file name is different due to using timestamps
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                //display the file urls
                Console.WriteLine(string.Join("\n\n", Enumerable.Select<ImageCreateResponse.ImageDataResult, string>(imageResult.Results, r => r.Url)));

                //open explorer to see the output files
                var uriString = Path.GetDirectoryName(path: Assembly.GetExecutingAssembly().CodeBase);
                _ = Process.Start("explorer.exe", "/open, " + new Uri(uriString).LocalPath);

                Console.WriteLine("\nPress any key to continue.");
                Console.ReadLine();
            }
        }

        private static async Task PromptChatGpt(IOpenAIService service, string prompt = "Who won the last three FIFA world cups?")
        {
            Console.WriteLine("Current prompt: " + prompt);
            Console.WriteLine("Enter prompt: ");

            var input = Console.ReadLine().ToLower();
            if (!string.IsNullOrEmpty(input))
            {
                prompt = input;
            }

            var completionResult = await service.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
                {
                    ChatMessage.FromUser(prompt)
                },
                Model = Models.ChatGpt3_5Turbo,
                MaxTokens = 150 //optional
            });

            Console.WriteLine(completionResult.Successful ? Enumerable.First<ChatChoiceResponse>(completionResult.Choices).Message.Content : $"{completionResult.Error.Code}: {completionResult.Error.Message}");

            Console.WriteLine("\nPress any key to continue.");
            Console.ReadLine();
        }

        private static int ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("Hello, ChatGPT world!");
            Console.WriteLine("==================================================");
            Console.WriteLine("Menu:                                             ");
            Console.WriteLine("1 - Ask ChatGPT a question.                       ");
            Console.WriteLine("2 - Ask DALL-E to generate image(s) from a prompt.");
            Console.WriteLine("Q - Quit.                                         ");
            Console.WriteLine("==================================================");
            Console.WriteLine("Enter Choice: ");

            var input = Console.ReadLine().ToLower();

            if (input.Contains('q'))
            {
                return 0;
            }
            else if (input.Contains('1') || input.Contains('2'))
            {
                return int.Parse(input);
            }

            ShowMenu();

            return 0;
        }
    }
}