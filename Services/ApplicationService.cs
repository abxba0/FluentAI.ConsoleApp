using FluentAI.Abstractions;
using FluentAI.Abstractions.Models;
using FluentAI.Configuration;
using FluentAI.ConsoleApp.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentAI.ConsoleApp.Services;

public interface IApplicationService
{
    Task RunAsync();
}

public class ApplicationService : IApplicationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IApiKeyConfigurationService _configService;

    public ApplicationService(IServiceProvider serviceProvider, IApiKeyConfigurationService configService)
    {
        _serviceProvider = serviceProvider;
        _configService = configService;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("FluentAI.NET Console Application Demo");
        Console.WriteLine("=====================================");

        var apiKey = _configService.GetApiKey();
        var model = _configService.GetModel();

        try
        {
            Console.WriteLine("FluentAI client initialized successfully!");
            Console.WriteLine($"Using API key: {apiKey.Substring(0, Math.Min(10, apiKey.Length))}...");
            Console.WriteLine($"Using model: {model}");

            // Get the chat model factory
            var chatModelFactory = _serviceProvider.GetRequiredService<IChatModelFactory>();
            var chatModel = chatModelFactory.GetModel("OpenAI");

            // Demonstrate simple prompt and response
            Console.WriteLine("\nSending a simple prompt to the AI...");
            var prompt = "Hello! Please introduce yourself briefly.";
            Console.WriteLine($"Prompt: {prompt}");

            // Create chat messages
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.User, prompt)
            };

            // Send prompt and get response
            var requestOptions = new OpenAiRequestOptions();
            var response = await chatModel.GetResponseAsync(messages, requestOptions, CancellationToken.None);

            Console.WriteLine($"\nAI Response: {response.Content}");
            Console.WriteLine($"Model ID: {response.ModelId}");
            Console.WriteLine($"Finish Reason: {response.FinishReason}");
            Console.WriteLine($"Token Usage: Input={response.Usage.InputTokens}, Output={response.Usage.OutputTokens}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError occurred: {ex.Message}");
            Console.WriteLine("\nNote: This demo requires a valid OpenAI API key to work properly.");
            Console.WriteLine("You can configure the API key in several ways:");
            Console.WriteLine("1. Set it in appsettings.json under Aisdk:OpenAI:ApiKey");
            Console.WriteLine("2. Set the OPENAI_API_KEY environment variable");
            Console.WriteLine("3. Use dotnet user-secrets (see README.md for details)");
            Console.WriteLine("\nTo get an API key:");
            Console.WriteLine("1. Visit https://platform.openai.com/api-keys");
            Console.WriteLine("2. Create an account or sign in");
            Console.WriteLine("3. Generate a new API key");
            Console.WriteLine("4. Configure it using one of the methods above");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}