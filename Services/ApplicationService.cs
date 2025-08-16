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
    private readonly IInteractiveChatService _chatService;

    public ApplicationService(
        IServiceProvider serviceProvider, 
        IApiKeyConfigurationService configService,
        IInteractiveChatService chatService)
    {
        _serviceProvider = serviceProvider;
        _configService = configService;
        _chatService = chatService;
    }

    public async Task RunAsync()
    {
        try
        {
            // Start the interactive chat session
            await _chatService.StartChatAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nUnexpected error: {ex.Message}");
            Console.WriteLine("\nNote: This application requires valid API keys to work properly.");
            Console.WriteLine("You can configure API keys in several ways:");
            Console.WriteLine("1. Set environment variables (OPENAI_API_KEY, GOOGLE_API_KEY)");
            Console.WriteLine("2. Use dotnet user-secrets (see README.md for details)");
            Console.WriteLine("3. For development only: Set in appsettings.json (not recommended for production)");
            Console.WriteLine("\nTo get API keys:");
            Console.WriteLine("- OpenAI: https://platform.openai.com/api-keys");
            Console.WriteLine("- Google: https://makersuite.google.com/app/apikey");
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}