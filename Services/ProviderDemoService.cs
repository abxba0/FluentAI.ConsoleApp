using FluentAI.Abstractions;
using FluentAI.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FluentAI.Examples.ConsoleApp;

/// <summary>
/// Demonstrates multi-provider capabilities and configuration-based provider switching.
/// </summary>
public class ProviderDemoService
{
    private readonly IChatModel _chatModel;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProviderDemoService> _logger;

    public ProviderDemoService(IChatModel chatModel, IServiceProvider serviceProvider, ILogger<ProviderDemoService> logger)
    {
        _chatModel = chatModel;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task RunProviderComparison()
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                Multi-Provider Comparison                    ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        var testPrompts = new[]
        {
            "Explain quantum computing in simple terms.",
            "Write a haiku about artificial intelligence.",
            "What are the key principles of object-oriented programming?"
        };

        Console.WriteLine("🧪 Testing different prompts with the configured provider...");
        Console.WriteLine();

        foreach (var prompt in testPrompts)
        {
            await TestProviderWithPrompt(prompt);
            Console.WriteLine();
            await Task.Delay(1000); // Small delay between tests
        }

        await ShowProviderSpecificFeatures();
    }

    private async Task TestProviderWithPrompt(string prompt)
    {
        Console.WriteLine($"🔍 Testing prompt: \"{TruncateText(prompt, 60)}\"");
        Console.WriteLine("   ──────────────────────────────────────────────────");

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a helpful assistant. Be concise and informative."),
            new(ChatRole.User, prompt)
        };

        try
        {
            var startTime = DateTime.UtcNow;
            var response = await _chatModel.GetResponseAsync(messages);
            var duration = DateTime.UtcNow - startTime;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   ✅ Response received in {duration.TotalMilliseconds:F0}ms");
            Console.ResetColor();
            
            Console.WriteLine($"   📝 Response: {TruncateText(response.Content, 120)}");
            Console.WriteLine($"   🧠 Model: {response.ModelId}");
            Console.WriteLine($"   📊 Tokens: {response.Usage.InputTokens} → {response.Usage.OutputTokens} (Total: {response.Usage.TotalTokens})");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"   ❌ Error: {ex.Message}");
            Console.ResetColor();
            _logger.LogWarning(ex, "Provider test failed for prompt: {Prompt}", prompt);
        }
    }

    private async Task ShowProviderSpecificFeatures()
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              Provider-Specific Features                     ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        Console.WriteLine("🔧 FluentAI.NET supports multiple AI providers with unified interface:");
        Console.WriteLine();

        // OpenAI specific features
        Console.WriteLine("🤖 OpenAI Features:");
        Console.WriteLine("   • GPT-3.5 Turbo, GPT-4, GPT-4o models");
        Console.WriteLine("   • Function calling capabilities");
        Console.WriteLine("   • Vision model support (GPT-4V)");
        Console.WriteLine("   • Advanced parameter controls (temperature, top_p, etc.)");
        Console.WriteLine("   • Streaming and batch processing");
        Console.WriteLine();

        // Anthropic specific features
        Console.WriteLine("🧠 Anthropic Features:");
        Console.WriteLine("   • Claude-3 family (Haiku, Sonnet, Opus)");
        Console.WriteLine("   • Constitutional AI safety mechanisms");
        Console.WriteLine("   • Large context windows (200K+ tokens)");
        Console.WriteLine("   • Advanced reasoning capabilities");
        Console.WriteLine("   • System prompt optimization");
        Console.WriteLine();

        // Google specific features
        Console.WriteLine("🌐 Google AI Features:");
        Console.WriteLine("   • Gemini Pro and Flash models");
        Console.WriteLine("   • Multimodal capabilities (text, images, code)");
        Console.WriteLine("   • Built-in safety filters");
        Console.WriteLine("   • Real-time streaming inference");
        Console.WriteLine("   • Integration with Google Cloud services");
        Console.WriteLine();

        // HuggingFace specific features
        Console.WriteLine("🤗 HuggingFace Features:");
        Console.WriteLine("   • Access to 100,000+ open-source models");
        Console.WriteLine("   • Community-driven model ecosystem");
        Console.WriteLine("   • Custom model deployment options");
        Console.WriteLine("   • Free tier with rate limits");
        Console.WriteLine("   • Support for specialized domain models");
        Console.WriteLine();

        Console.WriteLine("💡 Key Benefits:");
        Console.WriteLine("   ✅ Single interface (IChatModel) for all providers");
        Console.WriteLine("   ✅ Easy provider switching via configuration");
        Console.WriteLine("   ✅ Automatic failover between providers");
        Console.WriteLine("   ✅ Provider-agnostic application code");
        Console.WriteLine("   ✅ Consistent error handling across providers");

        await Task.CompletedTask;
    }

    private string TruncateText(string text, int maxLength)
    {
        if (text.Length <= maxLength)
            return text;
        
        return text.Substring(0, maxLength) + "...";
    }
}