using FluentAI.Abstractions;
using FluentAI.Abstractions.Models;
using FluentAI.Abstractions.Performance;
using FluentAI.Abstractions.Security;
using FluentAI.Abstractions.Exceptions;
using FluentAI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FluentAI.Examples.ConsoleApp;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    FluentAI.NET SDK Demo                    ║");
        Console.WriteLine("║                  Comprehensive Feature Showcase             ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        
        try
        {
            // Create host builder with comprehensive service configuration
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    try
                    {
                        var aiSdkSection = context.Configuration.GetSection("AiSdk");
                        Console.WriteLine($"AiSdk Section Exists: {aiSdkSection.Exists()}");
                        Console.WriteLine($"DefaultProvider: {aiSdkSection["DefaultProvider"]}");

                        // Register FluentAI with all providers and features
                        services.AddAiSdk(context.Configuration);
                        
                        // Add individual providers for direct access
                        services.AddOpenAiChatModel(context.Configuration);
                        services.AddGoogleGeminiChatModel(context.Configuration);
                        
                        
                        // Register application services
                        services.AddTransient<DemoService>();
                        services.AddTransient<ProviderDemoService>();
                        services.AddTransient<SecurityDemoService>();
                        services.AddTransient<PerformanceDemoService>();
                        services.AddTransient<ConfigurationDemoService>();
                        services.AddTransient<ErrorHandlingDemoService>();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Configuration Error: {ex.Message}");
                Console.ResetColor();
                
                // Log additional troubleshooting information
                Console.WriteLine();
                Console.WriteLine("💡 Troubleshooting:");
                Console.WriteLine("   • Ensure appsettings.json exists and contains an 'AiSdk' section");
                Console.WriteLine("   • Copy configuration from appsettings.example.json if needed");
                Console.WriteLine("   • Set 'AiSdk:DefaultProvider' to one of: 'OpenAI', 'Anthropic', 'Google', 'HuggingFace'");
                Console.WriteLine("   • Ensure required environment variables are set (e.g., OPENAI_API_KEY)");
                Console.WriteLine();
                
                if (args.Contains("--debug"))
                {
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                }
                
                Environment.Exit(1);
            }
        });

        using var host = builder.Build();
        // Validate that the service can be created before showing the menu
        try
        {
            var chatModel = host.Services.GetRequiredService<IChatModel>();
            Console.WriteLine("✅ FluentAI.NET SDK initialized successfully");
            Console.WriteLine();
        }
            catch (AiSdkConfigurationException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Configuration Error: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("💡 Troubleshooting:");
                Console.WriteLine("   • Check your appsettings.json for the 'AiSdk' section");
                Console.WriteLine("   • Copy configuration from appsettings.example.json if the section is missing");
                Console.WriteLine("   • Ensure 'AiSdk:DefaultProvider' is set to: 'OpenAI', 'Anthropic', 'Google', or 'HuggingFace'");
                Console.WriteLine("   • Verify that the specified provider is properly registered and configured");
                Console.WriteLine("   • Check that required environment variables are set (e.g., OPENAI_API_KEY)");
                Console.WriteLine();
                
                if (args.Contains("--debug"))
                {
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                }
                
                return;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Service Validation Error: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine();
                
                // Check if this is the known library issue
                if (ex.Message.Contains("Cannot create default input sanitizer without compatible logger"))
                {
                    Console.WriteLine("💡 Known Issue: This appears to be a compatibility issue with FluentAI.NET v1.0.2");
                    Console.WriteLine("   The AiSdk configuration is correct, but the library has a dependency injection issue.");
                    Console.WriteLine("   This typically happens when there's a type mismatch between ILogger<T> and ILogger.");
                    Console.WriteLine();
                    Console.WriteLine("🔧 Workarounds:");
                    Console.WriteLine("   • Try updating FluentAI.NET to a newer version if available");
                    Console.WriteLine("   • The configuration in appsettings.json is correct and will work with a compatible version");
                    Console.WriteLine("   • All required API keys are properly configured via user-secrets");
                }
                else
                {
                    Console.WriteLine("💡 This typically means your provider configuration is incomplete.");
                    Console.WriteLine("   Please check your appsettings.json and environment variables.");
                }
                Console.WriteLine();
                
                if (args.Contains("--debug"))
                {
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                }
                
                return;
            }
            
        var demoService = host.Services.GetRequiredService<DemoService>();
            await demoService.RunMainMenu();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ Application Error: {ex.Message}");
            Console.ResetColor();
            if (args.Contains("--debug"))
            {
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}

/// <summary>
/// Main demonstration service that provides an interactive menu to showcase all SDK features.
/// </summary>
public class DemoService
{
    private readonly IChatModel _chatModel;
    private readonly ILogger<DemoService> _logger;
    private readonly ProviderDemoService _providerDemo;
    private readonly SecurityDemoService _securityDemo;
    private readonly PerformanceDemoService _performanceDemo;
    private readonly ConfigurationDemoService _configDemo;
    private readonly ErrorHandlingDemoService _errorDemo;

    public DemoService(
        IChatModel chatModel,
        ILogger<DemoService> logger,
        ProviderDemoService providerDemo,
        SecurityDemoService securityDemo,
        PerformanceDemoService performanceDemo,
        ConfigurationDemoService configDemo,
        ErrorHandlingDemoService errorDemo)
    {
        _chatModel = chatModel;
        _logger = logger;
        _providerDemo = providerDemo;
        _securityDemo = securityDemo;
        _performanceDemo = performanceDemo;
        _configDemo = configDemo;
        _errorDemo = errorDemo;
    }

    public async Task RunMainMenu()
    {
        while (true)
        {
            DisplayMainMenu();
            
            var choice = Console.ReadLine()?.Trim();
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await RunBasicChatDemo();
                        break;
                    case "2":
                        await RunStreamingDemo();
                        break;
                    case "3":
                        await _providerDemo.RunProviderComparison();
                        break;
                    case "4":
                        await _securityDemo.RunSecurityFeatures();
                        break;
                    case "5":
                        await _performanceDemo.RunPerformanceDemo();
                        break;
                    case "6":
                        await _configDemo.RunConfigurationDemo();
                        break;
                    case "7":
                        await _errorDemo.RunErrorHandlingDemo();
                        break;
                    case "8":
                        await RunAdvancedFeaturesDemo();
                        break;
                    case "9":
                        await RunInteractiveChat();
                        break;
                    case "10":
                        await EdgeCaseTestService.RunEdgeCaseTests();
                        break;
                    case "0":
                    case "exit":
                    case "quit":
                        Console.WriteLine("👋 Thank you for exploring FluentAI.NET!");
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("❌ Invalid option. Please try again.");
                        Console.ResetColor();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.ResetColor();
                _logger.LogError(ex, "Error in demo operation");
            }

            if (choice != "0" && choice != "exit" && choice != "quit")
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    private void DisplayMainMenu()
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    FluentAI.NET SDK Demo                    ║");
        Console.WriteLine("║                     Main Feature Menu                       ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
        Console.WriteLine("║ 1. 💬 Basic Chat Completion Demo                            ║");
        Console.WriteLine("║ 2. 🌊 Streaming Response Demo                               ║");
        Console.WriteLine("║ 3. 🔄 Multi-Provider Comparison                             ║");
        Console.WriteLine("║ 4. 🔒 Security Features Demo                                ║");
        Console.WriteLine("║ 5. ⚡ Performance & Caching Demo                           ║");
        Console.WriteLine("║ 6. ⚙️ Configuration Management Demo                         ║");
        Console.WriteLine("║ 7. 🚨 Error Handling & Resilience Demo                     ║");
        Console.WriteLine("║ 8. 🔧 Advanced Features Demo                               ║");
        Console.WriteLine("║ 9. 💻 Interactive Chat (Original Demo)                     ║");
        Console.WriteLine("║ 10. 🧪 Edge Case & Error Scenario Tests                   ║");
        Console.WriteLine("║ 0. 🚪 Exit                                                  ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.Write("\nSelect an option (0-10): ");
    }

    private async Task RunBasicChatDemo()
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    Basic Chat Demo                          ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a helpful AI assistant. Be concise and informative."),
            new(ChatRole.User, "Explain what FluentAI.NET is in 2-3 sentences.")
        };

        Console.WriteLine("📤 Sending message to AI...");
        Console.WriteLine($"User: {messages.Last().Content}");
        Console.WriteLine();

        try
        {
            var response = await _chatModel.GetResponseAsync(messages);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🤖 AI Response:");
            Console.ResetColor();
            Console.WriteLine(response.Content);
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("📊 Response Metadata:");
            Console.WriteLine($"   Model: {response.ModelId}");
            Console.WriteLine($"   Input Tokens: {response.Usage.InputTokens}");
            Console.WriteLine($"   Output Tokens: {response.Usage.OutputTokens}");
            Console.WriteLine($"   Total Tokens: {response.Usage.TotalTokens}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.ResetColor();
        }
    }

    private async Task RunStreamingDemo()
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    Streaming Demo                           ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a creative storyteller. Tell engaging short stories."),
            new(ChatRole.User, "Tell me a short story about a robot who discovers emotions.")
        };

        Console.WriteLine("📤 Starting streaming response...");
        Console.WriteLine($"User: {messages.Last().Content}");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("🤖 AI Response (streaming): ");

        try
        {
            var fullResponse = "";
            await foreach (var token in _chatModel.StreamResponseAsync(messages))
            {
                Console.Write(token);
                fullResponse += token;
                await Task.Delay(10); // Small delay to visualize streaming
            }
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"📊 Total response length: {fullResponse.Length} characters");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ Streaming Error: {ex.Message}");
            Console.ResetColor();
        }
    }

    private async Task RunAdvancedFeaturesDemo()
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                  Advanced Features Demo                     ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        Console.WriteLine("🔧 Advanced features available in FluentAI.NET:");
        Console.WriteLine();
        Console.WriteLine("✅ Multi-provider support (OpenAI, Anthropic, Google, HuggingFace)");
        Console.WriteLine("✅ Automatic failover between providers");
        Console.WriteLine("✅ Rate limiting with sliding window");
        Console.WriteLine("✅ Response caching for performance");
        Console.WriteLine("✅ Input sanitization and security scanning");
        Console.WriteLine("✅ Performance monitoring and metrics");
        Console.WriteLine("✅ Structured logging and observability");
        Console.WriteLine("✅ Dependency injection integration");
        Console.WriteLine("✅ Configuration-based setup");
        Console.WriteLine("✅ Comprehensive error handling");
        Console.WriteLine("✅ Memory management and cleanup");
        Console.WriteLine("✅ Streaming responses");
        Console.WriteLine("✅ Custom provider extensions");
        Console.WriteLine();

        Console.WriteLine("💡 Use the other menu options to explore these features interactively!");
    }

    private async Task RunInteractiveChat()
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                   Interactive Chat                          ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("Type 'exit' to return to main menu, 'stream' to toggle streaming mode");
        Console.WriteLine();
        
        bool useStreaming = false;
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a helpful assistant. Keep responses concise.")
        };

        while (true)
        {
            Console.Write($"You ({(useStreaming ? "streaming" : "normal")}): ");
            var input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
                continue;
                
            if (input.ToLower() == "exit")
                break;
                
            if (input.ToLower() == "stream")
            {
                useStreaming = !useStreaming;
                Console.WriteLine($"Switched to {(useStreaming ? "streaming" : "normal")} mode");
                continue;
            }

            messages.Add(new ChatMessage(ChatRole.User, input));

            try
            {
                Console.Write("Assistant: ");
                
                if (useStreaming)
                {
                    var response = "";
                    await foreach (var token in _chatModel.StreamResponseAsync(messages))
                    {
                        Console.Write(token);
                        response += token;
                    }
                    Console.WriteLine();
                    messages.Add(new ChatMessage(ChatRole.Assistant, response));
                }
                else
                {
                    var response = await _chatModel.GetResponseAsync(messages);
                    Console.WriteLine(response.Content);
                    Console.WriteLine($"[Model: {response.ModelId}, Tokens: {response.Usage.InputTokens}→{response.Usage.OutputTokens}]");
                    messages.Add(new ChatMessage(ChatRole.Assistant, response.Content));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during chat completion");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
            
            Console.WriteLine();
        }
    }
}