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

        var aiSdkConfig = _configService.GetAiSdkConfiguration();
        var defaultProvider = _configService.GetDefaultProvider();
        var primaryProvider = _configService.GetPrimaryProvider();
        var fallbackProvider = _configService.GetFallbackProvider();

        Console.WriteLine($"Default Provider: {defaultProvider}");
        Console.WriteLine($"Primary Provider: {primaryProvider}");
        Console.WriteLine($"Fallback Provider: {fallbackProvider}");

        var apiKey = _configService.GetApiKey(defaultProvider);
        var model = _configService.GetModel(defaultProvider);
        var providerConfig = _configService.GetProviderConfiguration(defaultProvider);

        Console.WriteLine($"Provider Configuration:");
        Console.WriteLine($"- Model: {providerConfig.Model}");
        Console.WriteLine($"- Max Tokens: {providerConfig.MaxTokens}");
        Console.WriteLine($"- Request Timeout: {providerConfig.RequestTimeout}");
        Console.WriteLine($"- Permit Limit: {providerConfig.PermitLimit}");
        Console.WriteLine($"- Window In Seconds: {providerConfig.WindowInSeconds}");

        try
        {
            Console.WriteLine("FluentAI client initialized successfully!");
            Console.WriteLine($"Using API key: {apiKey.Substring(0, Math.Min(10, apiKey.Length))}...");
            Console.WriteLine($"Using model: {model}");

            // Try primary provider first
            var success = await TryProvider(primaryProvider, "Primary");
            
            // If primary fails, try fallback
            if (!success && !primaryProvider.Equals(fallbackProvider, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"\nPrimary provider ({primaryProvider}) failed, trying fallback provider ({fallbackProvider})...");
                await TryProvider(fallbackProvider, "Fallback");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError occurred: {ex.Message}");
            Console.WriteLine("\nNote: This demo requires valid API keys to work properly.");
            Console.WriteLine("You can configure API keys in several ways:");
            Console.WriteLine("1. Set environment variables (OPENAI_API_KEY, GOOGLE_API_KEY)");
            Console.WriteLine("2. Use dotnet user-secrets (see README.md for details)");
            Console.WriteLine("3. For development only: Set in appsettings.json (not recommended for production)");
            Console.WriteLine("\nTo get API keys:");
            Console.WriteLine("- OpenAI: https://platform.openai.com/api-keys");
            Console.WriteLine("- Google: https://makersuite.google.com/app/apikey");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private async Task<bool> TryProvider(string provider, string providerType)
    {
        try
        {
            Console.WriteLine($"\n=== Trying {providerType} Provider: {provider} ===");
            
            // Get the chat model factory
            var chatModelFactory = _serviceProvider.GetRequiredService<IChatModelFactory>();
            var chatModel = chatModelFactory.GetModel(provider);

            // Demonstrate simple prompt and response
            Console.WriteLine("Sending a simple prompt to the AI...");
            var prompt = "Hello! Please introduce yourself briefly.";
            Console.WriteLine($"Prompt: {prompt}");

            // Create chat messages
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.User, prompt)
            };

            // Send prompt and get response
            var requestOptions = new OpenAiRequestOptions(); // This might need to be provider-specific
            var response = await chatModel.GetResponseAsync(messages, requestOptions, CancellationToken.None);

            Console.WriteLine($"\nAI Response: {response.Content}");
            Console.WriteLine($"Model ID: {response.ModelId}");
            Console.WriteLine($"Finish Reason: {response.FinishReason}");
            Console.WriteLine($"Token Usage: Input={response.Usage.InputTokens}, Output={response.Usage.OutputTokens}");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{providerType} provider ({provider}) failed: {ex.Message}");
            return false;
        }
    }
}