using FluentAI.ConsoleApp.Configuration;
using Microsoft.Extensions.Configuration;

namespace FluentAI.ConsoleApp.Tests;

/// <summary>
/// Simple manual test to validate configuration loading
/// Run with: dotnet run --project . -- test-config
/// </summary>
public static class ConfigurationTest
{
    public static void TestConfiguration()
    {
        Console.WriteLine("=== Configuration Loading Test ===");
        
        // Build configuration the same way as in Program.cs
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var configService = new ApiKeyConfigurationService(configuration);

        // Test AiSdk configuration
        var aiSdkConfig = configService.GetAiSdkConfiguration();
        Console.WriteLine($"✓ Default Provider: {aiSdkConfig.DefaultProvider}");
        Console.WriteLine($"✓ Primary Provider: {aiSdkConfig.Failover.PrimaryProvider}");
        Console.WriteLine($"✓ Fallback Provider: {aiSdkConfig.Failover.FallbackProvider}");

        // Test OpenAI configuration
        var openAiConfig = configService.GetProviderConfiguration("OpenAI");
        Console.WriteLine($"✓ OpenAI Model: {openAiConfig.Model}");
        Console.WriteLine($"✓ OpenAI Max Tokens: {openAiConfig.MaxTokens}");
        Console.WriteLine($"✓ OpenAI Request Timeout: {openAiConfig.RequestTimeout}");
        Console.WriteLine($"✓ OpenAI Permit Limit: {openAiConfig.PermitLimit}");
        Console.WriteLine($"✓ OpenAI Window In Seconds: {openAiConfig.WindowInSeconds}");

        // Test Google configuration
        var googleConfig = configService.GetProviderConfiguration("Google");
        Console.WriteLine($"✓ Google Model: {googleConfig.Model}");
        Console.WriteLine($"✓ Google Max Tokens: {googleConfig.MaxTokens}");
        Console.WriteLine($"✓ Google Request Timeout: {googleConfig.RequestTimeout}");
        Console.WriteLine($"✓ Google Permit Limit: {googleConfig.PermitLimit}");
        Console.WriteLine($"✓ Google Window In Seconds: {googleConfig.WindowInSeconds}");

        // Test API key resolution (should use placeholders)
        var openAiApiKey = configService.GetApiKey("OpenAI");
        var googleApiKey = configService.GetApiKey("Google");
        Console.WriteLine($"✓ OpenAI API Key: {openAiApiKey.Substring(0, Math.Min(15, openAiApiKey.Length))}...");
        Console.WriteLine($"✓ Google API Key: {googleApiKey.Substring(0, Math.Min(15, googleApiKey.Length))}...");

        // Test model resolution
        var openAiModel = configService.GetModel("OpenAI");
        var googleModel = configService.GetModel("Google");
        Console.WriteLine($"✓ OpenAI Model via GetModel: {openAiModel}");
        Console.WriteLine($"✓ Google Model via GetModel: {googleModel}");

        Console.WriteLine("=== All Configuration Tests Passed! ===");
    }
}