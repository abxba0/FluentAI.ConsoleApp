using FluentAI.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentAI.Examples.ConsoleApp;

/// <summary>
/// Demonstrates configuration management and different setup approaches.
/// </summary>
public class ConfigurationDemoService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ConfigurationDemoService> _logger;

    public ConfigurationDemoService(
        IConfiguration configuration,
        IServiceProvider serviceProvider,
        ILogger<ConfigurationDemoService> logger)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task RunConfigurationDemo()
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                Configuration Management Demo                 ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        await ShowCurrentConfiguration();
        Console.WriteLine();
        await ShowConfigurationSources();
        Console.WriteLine();
        await ShowProviderConfiguration();
        Console.WriteLine();
        await ShowEnvironmentVariableUsage();
        Console.WriteLine();
        await ShowConfigurationValidation();
    }

    private async Task ShowCurrentConfiguration()
    {
        Console.WriteLine("⚙️ Current Configuration:");
        Console.WriteLine("   ─────────────────────");
        Console.WriteLine();

        try
        {
            // Show AI SDK options
            var aiSdkOptions = _serviceProvider.GetService<IOptions<AiSdkOptions>>();
            if (aiSdkOptions?.Value != null)
            {
                var options = aiSdkOptions.Value;
                Console.WriteLine("🔧 AI SDK Configuration:");
                Console.WriteLine($"   • Default Provider: {options.DefaultProvider ?? "Not set"}");
                
                if (options.Failover != null)
                {
                    Console.WriteLine($"   • Failover Primary: {options.Failover.PrimaryProvider}");
                    Console.WriteLine($"   • Failover Fallback: {options.Failover.FallbackProvider}");
                }
                else
                {
                    Console.WriteLine("   • Failover: Not configured");
                }
            }

            Console.WriteLine();

            // Show OpenAI configuration
            var openAiOptions = _serviceProvider.GetService<IOptions<OpenAiOptions>>();
            if (openAiOptions?.Value != null)
            {
                var options = openAiOptions.Value;
                Console.WriteLine("🤖 OpenAI Configuration:");
                Console.WriteLine($"   • Model: {options.Model ?? "Not set"}");
                Console.WriteLine($"   • Max Tokens: {options.MaxTokens}");
                Console.WriteLine($"   • API Key: {(string.IsNullOrEmpty(options.ApiKey) ? "Not set" : "****" + options.ApiKey[^4..])}");
                Console.WriteLine($"   • Request Timeout: {options.RequestTimeout}");
                
                if (options.PermitLimit.HasValue)
                {
                    Console.WriteLine($"   • Rate Limit: {options.PermitLimit} requests per {options.WindowInSeconds} seconds");
                }
            }

            Console.WriteLine();

            // Show Anthropic configuration
            var anthropicOptions = _serviceProvider.GetService<IOptions<AnthropicOptions>>();
            if (anthropicOptions?.Value != null)
            {
                var options = anthropicOptions.Value;
                Console.WriteLine("🧠 Anthropic Configuration:");
                Console.WriteLine($"   • Model: {options.Model ?? "Not set"}");
                Console.WriteLine($"   • Max Tokens: {options.MaxTokens}");
                Console.WriteLine($"   • API Key: {(string.IsNullOrEmpty(options.ApiKey) ? "Not set" : "****" + options.ApiKey[^4..])}");
                Console.WriteLine($"   • Request Timeout: {options.RequestTimeout}");
                
                if (options.PermitLimit.HasValue)
                {
                    Console.WriteLine($"   • Rate Limit: {options.PermitLimit} requests per {options.WindowInSeconds} seconds");
                }
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error reading configuration: {ex.Message}");
            Console.ResetColor();
        }

        await Task.CompletedTask;
    }

    private async Task ShowConfigurationSources()
    {
        Console.WriteLine("📁 Configuration Sources:");
        Console.WriteLine("   ─────────────────────");
        Console.WriteLine();

        Console.WriteLine("FluentAI.NET supports multiple configuration sources:");
        Console.WriteLine();

        Console.WriteLine("1. 📄 appsettings.json:");
        Console.WriteLine("   {");
        Console.WriteLine("     \"AiSdk\": {");
        Console.WriteLine("       \"DefaultProvider\": \"OpenAI\"");
        Console.WriteLine("     },");
        Console.WriteLine("     \"OpenAI\": {");
        Console.WriteLine("       \"Model\": \"gpt-3.5-turbo\",");
        Console.WriteLine("       \"MaxTokens\": 1000");
        Console.WriteLine("     }");
        Console.WriteLine("   }");
        Console.WriteLine();

        Console.WriteLine("2. 🌍 Environment Variables:");
        Console.WriteLine("   export OPENAI_API_KEY=\"your-key\"");
        Console.WriteLine("   export ANTHROPIC_API_KEY=\"your-key\"");
        Console.WriteLine();

        Console.WriteLine("3. 💻 Programmatic Configuration:");
        Console.WriteLine("   services.AddFluentAI()");
        Console.WriteLine("     .AddOpenAI(config => {");
        Console.WriteLine("       config.ApiKey = \"your-key\";");
        Console.WriteLine("       config.Model = \"gpt-4\";");
        Console.WriteLine("     });");
        Console.WriteLine();

        Console.WriteLine("4. ☁️ Azure Key Vault / AWS Secrets Manager");
        Console.WriteLine("5. 🔧 Command Line Arguments");
        Console.WriteLine("6. 📊 User Secrets (Development)");

        await Task.CompletedTask;
    }

    private async Task ShowProviderConfiguration()
    {
        Console.WriteLine("🔌 Provider-Specific Configuration:");
        Console.WriteLine("   ─────────────────────────────────");
        Console.WriteLine();

        Console.WriteLine("Each provider can be configured independently:");
        Console.WriteLine();

        Console.WriteLine("🤖 OpenAI Provider:");
        Console.WriteLine("   • ApiKey: Your OpenAI API key");
        Console.WriteLine("   • Model: gpt-3.5-turbo, gpt-4, gpt-4o, etc.");
        Console.WriteLine("   • MaxTokens: Maximum response tokens");
        Console.WriteLine("   • Temperature: Response creativity (0.0-2.0)");
        Console.WriteLine("   • RequestTimeout: API request timeout");
        Console.WriteLine("   • PermitLimit: Rate limiting configuration");
        Console.WriteLine();

        Console.WriteLine("🧠 Anthropic Provider:");
        Console.WriteLine("   • ApiKey: Your Anthropic API key");
        Console.WriteLine("   • Model: claude-3-sonnet, claude-3-haiku, etc.");
        Console.WriteLine("   • MaxTokens: Maximum response tokens");
        Console.WriteLine("   • SystemPrompt: System behavior instructions");
        Console.WriteLine("   • RequestTimeout: API request timeout");
        Console.WriteLine("   • PermitLimit: Rate limiting configuration");
        Console.WriteLine();

        Console.WriteLine("🌐 Google Provider:");
        Console.WriteLine("   • ApiKey: Your Google AI API key");
        Console.WriteLine("   • Model: gemini-pro, gemini-flash, etc.");
        Console.WriteLine("   • MaxTokens: Maximum response tokens");
        Console.WriteLine("   • SafetySettings: Content safety controls");
        Console.WriteLine();

        Console.WriteLine("🤗 HuggingFace Provider:");
        Console.WriteLine("   • ApiKey: Your HuggingFace API key");
        Console.WriteLine("   • ModelId: Specific model identifier");
        Console.WriteLine("   • MaxTokens: Maximum response tokens");
        Console.WriteLine("   • UseInferenceApi: Use hosted inference");

        await Task.CompletedTask;
    }

    private async Task ShowEnvironmentVariableUsage()
    {
        Console.WriteLine("🌍 Environment Variable Usage:");
        Console.WriteLine("   ──────────────────────────────");
        Console.WriteLine();

        var envVars = new[]
        {
            "OPENAI_API_KEY",
            "ANTHROPIC_API_KEY", 
            "GOOGLE_API_KEY",
            "HUGGINGFACE_API_KEY"
        };

        Console.WriteLine("🔍 Checking environment variables:");
        Console.WriteLine();

        foreach (var envVar in envVars)
        {
            var value = Environment.GetEnvironmentVariable(envVar);
            var status = !string.IsNullOrEmpty(value) ? "✅ Set" : "❌ Not set";
            var color = !string.IsNullOrEmpty(value) ? ConsoleColor.Green : ConsoleColor.Red;
            
            Console.ForegroundColor = color;
            Console.WriteLine($"   {envVar}: {status}");
            Console.ResetColor();
            
            if (!string.IsNullOrEmpty(value))
            {
                Console.WriteLine($"      Value: ****{value[^Math.Min(4, value.Length)..]}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("💡 Best Practices:");
        Console.WriteLine("   • Store API keys in environment variables for security");
        Console.WriteLine("   • Use different keys for development/staging/production");
        Console.WriteLine("   • Never commit API keys to source control");
        Console.WriteLine("   • Consider using Azure Key Vault or similar for production");

        await Task.CompletedTask;
    }

    private async Task ShowConfigurationValidation()
    {
        Console.WriteLine("✅ Configuration Validation:");
        Console.WriteLine("   ─────────────────────────");
        Console.WriteLine();

        Console.WriteLine("FluentAI.NET validates configuration at startup:");
        Console.WriteLine();

        try
        {
            // Simulate configuration validation
            var issues = new List<string>();

            // Check for API keys
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPENAI_API_KEY")))
            {
                issues.Add("OpenAI API key not configured");
            }

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")))
            {
                issues.Add("Anthropic API key not configured");
            }

            // Check configuration values
            var defaultProvider = _configuration["AiSdk:DefaultProvider"];
            if (string.IsNullOrEmpty(defaultProvider))
            {
                issues.Add("Default provider not specified");
            }

            Console.WriteLine("🔍 Configuration Issues Found:");
            if (issues.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (var issue in issues)
                {
                    Console.WriteLine($"   ⚠️ {issue}");
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("   ✅ No configuration issues detected!");
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.WriteLine("🛠️ Validation Features:");
            Console.WriteLine("   • Required field validation");
            Console.WriteLine("   • API key format checking");
            Console.WriteLine("   • Model name validation");
            Console.WriteLine("   • Timeout range validation");
            Console.WriteLine("   • Rate limiting parameter validation");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Validation error: {ex.Message}");
            Console.ResetColor();
        }

        await Task.CompletedTask;
    }
}