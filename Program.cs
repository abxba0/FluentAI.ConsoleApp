using FluentAI.ConsoleApp.Configuration;
using FluentAI.ConsoleApp.Services;
using FluentAI.ConsoleApp.Tests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// FluentAI.NET Console Application Demo
// This demonstrates basic usage of the FluentAI.NET library

class Program
{
    static async Task Main(string[] args)
    {
        // Check for test arguments
        if (args.Length > 0)
        {
            switch (args[0])
            {
                case "test-config":
                    ConfigurationTest.TestConfiguration();
                    return;
                case "test-legacy":
                    BackwardCompatibilityTest.TestLegacyConfiguration();
                    return;
                case "test-features":
                    AiAssistantFeaturesTest.RunAllTests();
                    return;
                case "demo":
                    InteractiveChatDemo.RunDemo();
                    return;
            }
        }

        // Build configuration
        var configuration = BuildConfiguration();
        
        // Create service collection and configure dependencies
        var services = new ServiceCollection();
        ConfigureServices(services, configuration);
        
        // Build service provider
        var serviceProvider = services.BuildServiceProvider();
        
        // Run the application
        var app = serviceProvider.GetRequiredService<IApplicationService>();
        await app.RunAsync();
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>()
            .Build();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add configuration
        services.AddSingleton(configuration);
        
        // Add logging services using configuration from appsettings.json
        services.AddLogging(builder => 
        {
            builder.AddConsole();
            builder.AddConfiguration(configuration.GetSection("Logging"));
        });
        
        // Add custom services
        services.AddSingleton<IApiKeyConfigurationService, ApiKeyConfigurationService>();
        services.AddSingleton<IApplicationService, ApplicationService>();
        
        // Add new AI assistant services
        services.AddSingleton<IConversationManager, ConversationManager>();
        services.AddSingleton<IChatSessionManager, ChatSessionManager>();
        services.AddSingleton<IInputValidationService, InputValidationService>();
        services.AddSingleton<IInteractiveChatService, InteractiveChatService>();
        
        // Add FluentAI services using configuration
        var configService = new ApiKeyConfigurationService(configuration);
        services.AddFluentAiServices(configService);
    }
}
