using FluentAI.Abstractions;
using FluentAI.Abstractions.Models;
using FluentAI.Abstractions.Exceptions;
using Microsoft.Extensions.Logging;

namespace FluentAI.Examples.ConsoleApp;

/// <summary>
/// Demonstrates error handling, resilience features, and recovery mechanisms.
/// </summary>
public class ErrorHandlingDemoService
{
    private readonly IChatModel _chatModel;
    private readonly ILogger<ErrorHandlingDemoService> _logger;

    public ErrorHandlingDemoService(IChatModel chatModel, ILogger<ErrorHandlingDemoService> logger)
    {
        _chatModel = chatModel;
        _logger = logger;
    }

    public async Task RunErrorHandlingDemo()
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                Error Handling & Resilience Demo             ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        await RunBasicErrorHandling();
        Console.WriteLine();
        await RunRetryMechanismDemo();
        Console.WriteLine();
        await RunRateLimitHandling();
        Console.WriteLine();
        await RunFailoverDemo();
        Console.WriteLine();
        await RunValidationErrorDemo();
        Console.WriteLine();
        await ShowBestPractices();
    }

    private async Task RunBasicErrorHandling()
    {
        Console.WriteLine("🚨 Basic Error Handling Demo:");
        Console.WriteLine("   ─────────────────────────");
        Console.WriteLine();

        var errorScenarios = new[]
        {
            new { Name = "Valid Request", Messages = CreateValidMessages(), ShouldFail = false },
            new { Name = "Empty Message", Messages = CreateEmptyMessages(), ShouldFail = true },
            new { Name = "Oversized Request", Messages = CreateOversizedMessages(), ShouldFail = true }
        };

        foreach (var scenario in errorScenarios)
        {
            Console.WriteLine($"🧪 Testing: {scenario.Name}");
            
            try
            {
                var response = await _chatModel.GetResponseAsync(scenario.Messages);
                
                if (scenario.ShouldFail)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("   ⚠️ Expected error but request succeeded");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("   ✅ Request completed successfully");
                    Console.ResetColor();
                }
                
                Console.WriteLine($"   📝 Response length: {response.Content.Length} characters");
            }
            catch (AiSdkConfigurationException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"   ❌ Configuration Error: {ex.Message}");
                Console.ResetColor();
                _logger.LogError(ex, "Configuration error in scenario {Scenario}", scenario.Name);
            }
            catch (AiSdkRateLimitException ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   🚫 Rate Limit Error: {ex.Message}");
                Console.ResetColor();
                _logger.LogWarning(ex, "Rate limit hit in scenario {Scenario}", scenario.Name);
            }
            catch (AiSdkException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"   ❌ AI SDK Error: {ex.Message}");
                Console.ResetColor();
                _logger.LogError(ex, "SDK error in scenario {Scenario}", scenario.Name);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"   💥 Unexpected Error: {ex.Message}");
                Console.ResetColor();
                _logger.LogError(ex, "Unexpected error in scenario {Scenario}", scenario.Name);
            }

            Console.WriteLine();
            await Task.Delay(500); // Brief delay between tests
        }
    }

    private async Task RunRetryMechanismDemo()
    {
        Console.WriteLine("🔄 Retry Mechanism Demo:");
        Console.WriteLine("   ──────────────────────");
        Console.WriteLine();

        Console.WriteLine("FluentAI.NET includes automatic retry logic for transient failures:");
        Console.WriteLine();

        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, "What are the benefits of retry mechanisms in distributed systems?")
        };

        Console.WriteLine("📤 Making request with retry simulation...");
        
        var maxRetries = 3;
        var retryCount = 0;

        while (retryCount <= maxRetries)
        {
            try
            {
                Console.WriteLine($"🔄 Attempt {retryCount + 1}/{maxRetries + 1}:");
                
                var response = await _chatModel.GetResponseAsync(messages);
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("   ✅ Request successful!");
                Console.ResetColor();
                Console.WriteLine($"   📝 Response: {TruncateText(response.Content, 100)}");
                break;
            }
            catch (Exception ex) when (IsRetriableError(ex) && retryCount < maxRetries)
            {
                retryCount++;
                var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount)); // Exponential backoff
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   ⚠️ Transient error: {ex.Message}");
                Console.WriteLine($"   ⏳ Retrying in {delay.TotalSeconds} seconds...");
                Console.ResetColor();
                
                await Task.Delay(delay);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"   ❌ Non-retriable error: {ex.Message}");
                Console.ResetColor();
                break;
            }
        }

        if (retryCount > maxRetries)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ All retry attempts exhausted");
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.WriteLine("🔧 Retry Strategy Features:");
        Console.WriteLine("   • Exponential backoff with jitter");
        Console.WriteLine("   • Configurable retry counts and delays");
        Console.WriteLine("   • Intelligent error classification");
        Console.WriteLine("   • Circuit breaker pattern support");
    }

    private async Task RunRateLimitHandling()
    {
        Console.WriteLine("🚫 Rate Limit Handling Demo:");
        Console.WriteLine("   ─────────────────────────");
        Console.WriteLine();

        Console.WriteLine("FluentAI.NET includes built-in rate limiting to prevent API abuse:");
        Console.WriteLine();

        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, "Explain rate limiting in APIs.")
        };

        try
        {
            Console.WriteLine("📊 Simulating rate limit scenario...");
            
            // Make rapid requests to potentially trigger rate limiting
            for (int i = 1; i <= 3; i++)
            {
                Console.WriteLine($"🔄 Request {i}:");
                
                try
                {
                    var response = await _chatModel.GetResponseAsync(messages);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"   ✅ Success: {TruncateText(response.Content, 50)}");
                    Console.ResetColor();
                }
                catch (AiSdkRateLimitException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"   🚫 Rate limited: {ex.Message}");
                    Console.ResetColor();
                    
                    // In a real application, you would wait and retry
                    break;
                }
                
                await Task.Delay(100); // Small delay between requests
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.WriteLine("⚙️ Rate Limiting Features:");
        Console.WriteLine("   • Sliding window rate limiting");
        Console.WriteLine("   • Configurable permit limits");
        Console.WriteLine("   • Automatic retry with backoff");
        Console.WriteLine("   • Provider-specific rate limits");
        Console.WriteLine("   • Queue-based request handling");
    }

    private async Task RunFailoverDemo()
    {
        Console.WriteLine("🔄 Failover Demo:");
        Console.WriteLine("   ──────────────");
        Console.WriteLine();

        Console.WriteLine("FluentAI.NET supports automatic failover between providers:");
        Console.WriteLine();

        Console.WriteLine("🔧 Failover Configuration Example:");
        Console.WriteLine("   {");
        Console.WriteLine("     \"AiSdk\": {");
        Console.WriteLine("       \"Failover\": {");
        Console.WriteLine("         \"PrimaryProvider\": \"OpenAI\",");
        Console.WriteLine("         \"FallbackProvider\": \"Anthropic\"");
        Console.WriteLine("       }");
        Console.WriteLine("     }");
        Console.WriteLine("   }");
        Console.WriteLine();

        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, "Explain the benefits of failover mechanisms.")
        };

        Console.WriteLine("📤 Testing failover behavior...");
        
        try
        {
            var response = await _chatModel.GetResponseAsync(messages);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ Request completed (may have used failover)");
            Console.ResetColor();
            Console.WriteLine($"📝 Response: {TruncateText(response.Content, 100)}");
            Console.WriteLine($"🧠 Model: {response.ModelId}");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ All providers failed: {ex.Message}");
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.WriteLine("🌟 Failover Features:");
        Console.WriteLine("   • Automatic provider switching");
        Console.WriteLine("   • Configurable fallback chains");
        Console.WriteLine("   • Health check integration");
        Console.WriteLine("   • Transparent operation");
        Console.WriteLine("   • Performance monitoring");

        await Task.CompletedTask;
    }

    private async Task RunValidationErrorDemo()
    {
        Console.WriteLine("✅ Input Validation Demo:");
        Console.WriteLine("   ──────────────────────");
        Console.WriteLine();

        var validationTests = new[]
        {
            new { Name = "Null Messages", Messages = (IEnumerable<ChatMessage>?)null },
            new { Name = "Empty Messages", Messages = (IEnumerable<ChatMessage>)new List<ChatMessage>() },
            new { Name = "Valid Messages", Messages = (IEnumerable<ChatMessage>)new List<ChatMessage> { new(ChatRole.User, "Hello") } }
        };

        foreach (var test in validationTests)
        {
            Console.WriteLine($"🧪 Testing: {test.Name}");
            
            try
            {
                if (test.Messages == null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("   ⚠️ Skipping null message test for safety");
                    Console.ResetColor();
                    continue;
                }

                var response = await _chatModel.GetResponseAsync(test.Messages);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("   ✅ Validation passed");
                Console.ResetColor();
            }
            catch (ArgumentException ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   ⚠️ Validation Error: {ex.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"   ❌ Unexpected Error: {ex.Message}");
                Console.ResetColor();
            }
            
            Console.WriteLine();
        }
    }

    private async Task ShowBestPractices()
    {
        Console.WriteLine("💡 Error Handling Best Practices:");
        Console.WriteLine("   ────────────────────────────────");
        Console.WriteLine();

        Console.WriteLine("🛠️ Implementation Guidelines:");
        Console.WriteLine();

        Console.WriteLine("1. 🎯 Specific Exception Handling:");
        Console.WriteLine("   try {");
        Console.WriteLine("     var response = await chatModel.GetResponseAsync(messages);");
        Console.WriteLine("   }");
        Console.WriteLine("   catch (AiSdkRateLimitException ex) {");
        Console.WriteLine("     // Handle rate limiting with backoff");
        Console.WriteLine("   }");
        Console.WriteLine("   catch (AiSdkConfigurationException ex) {");
        Console.WriteLine("     // Handle configuration issues");
        Console.WriteLine("   }");
        Console.WriteLine();

        Console.WriteLine("2. ⏳ Retry with Exponential Backoff:");
        Console.WriteLine("   • Start with short delays (1-2 seconds)");
        Console.WriteLine("   • Double delay on each retry");
        Console.WriteLine("   • Add jitter to prevent thundering herd");
        Console.WriteLine("   • Set maximum retry limits");
        Console.WriteLine();

        Console.WriteLine("3. 📊 Monitoring and Logging:");
        Console.WriteLine("   • Log all errors with context");
        Console.WriteLine("   • Track error rates and patterns");
        Console.WriteLine("   • Monitor provider health");
        Console.WriteLine("   • Set up alerting for failures");
        Console.WriteLine();

        Console.WriteLine("4. 🔄 Graceful Degradation:");
        Console.WriteLine("   • Provide fallback responses");
        Console.WriteLine("   • Cache responses when possible");
        Console.WriteLine("   • Use circuit breaker patterns");
        Console.WriteLine("   • Implement timeout policies");
        Console.WriteLine();

        Console.WriteLine("5. 🔒 Security Considerations:");
        Console.WriteLine("   • Don't expose internal errors to users");
        Console.WriteLine("   • Sanitize error messages");
        Console.WriteLine("   • Rate limit error responses");
        Console.WriteLine("   • Log security-related errors");

        await Task.CompletedTask;
    }

    private List<ChatMessage> CreateValidMessages()
    {
        return new List<ChatMessage>
        {
            new(ChatRole.User, "Hello, how are you?")
        };
    }

    private List<ChatMessage> CreateEmptyMessages()
    {
        return new List<ChatMessage>
        {
            new(ChatRole.User, "")
        };
    }

    private List<ChatMessage> CreateOversizedMessages()
    {
        var largeContent = new string('A', 50000); // Very large message
        return new List<ChatMessage>
        {
            new(ChatRole.User, largeContent)
        };
    }

    private bool IsRetriableError(Exception ex)
    {
        return ex is AiSdkRateLimitException ||
               ex is HttpRequestException ||
               ex is TaskCanceledException ||
               (ex is AiSdkException && ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase));
    }

    private string TruncateText(string text, int maxLength)
    {
        if (text.Length <= maxLength)
            return text;
        
        return text.Substring(0, maxLength) + "...";
    }
}