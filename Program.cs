using FluentAI.ConsoleApp.Configuration;
using FluentAI.ConsoleApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// FluentAI.NET Console Application Demo
// This demonstrates basic usage of the FluentAI.NET library

class Program
{
    static async Task Main(string[] args)
    {
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
        
        // Add logging services
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        
        // Add custom services
        services.AddSingleton<IApiKeyConfigurationService, ApiKeyConfigurationService>();
        services.AddSingleton<IApplicationService, ApplicationService>();
        
        // Add FluentAI services using configuration
        var configService = new ApiKeyConfigurationService(configuration);
        services.AddFluentAiServices(configService);
    }
}
