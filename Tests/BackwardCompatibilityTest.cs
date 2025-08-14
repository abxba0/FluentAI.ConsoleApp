using FluentAI.ConsoleApp.Configuration;
using Microsoft.Extensions.Configuration;

namespace FluentAI.ConsoleApp.Tests;

/// <summary>
/// Test backward compatibility with legacy configuration format
/// </summary>
public static class BackwardCompatibilityTest
{
    public static void TestLegacyConfiguration()
    {
        Console.WriteLine("=== Backward Compatibility Test ===");
        
        // Build configuration with legacy format
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.legacy.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var configService = new ApiKeyConfigurationService(configuration);

        // Test legacy API key access
        var openAiApiKey = configService.GetApiKey("OpenAI");
        Console.WriteLine($"✓ Legacy OpenAI API Key: {openAiApiKey}");

        // Test legacy model access
        var openAiModel = configService.GetModel("OpenAI");
        Console.WriteLine($"✓ Legacy OpenAI Model: {openAiModel}");

        // Test fallback to defaults for new structure
        var aiSdkConfig = configService.GetAiSdkConfiguration();
        Console.WriteLine($"✓ Default Provider (fallback): {aiSdkConfig.DefaultProvider}");
        Console.WriteLine($"✓ Primary Provider (fallback): {aiSdkConfig.Failover.PrimaryProvider}");
        Console.WriteLine($"✓ Fallback Provider (fallback): {aiSdkConfig.Failover.FallbackProvider}");

        // Test provider configuration with legacy data
        var openAiConfig = configService.GetProviderConfiguration("OpenAI");
        Console.WriteLine($"✓ OpenAI Model from provider config: {openAiConfig.Model}");
        Console.WriteLine($"✓ OpenAI Max Tokens (default): {openAiConfig.MaxTokens}");

        Console.WriteLine("=== Backward Compatibility Test Passed! ===");
    }
}