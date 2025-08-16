using Microsoft.Extensions.Configuration;

namespace FluentAI.ConsoleApp.Configuration;

public class AiSdkConfiguration
{
    public string DefaultProvider { get; set; } = "OpenAI";
    public FailoverConfiguration Failover { get; set; } = new();
    public ChatConfiguration Chat { get; set; } = new();
}

public class ChatConfiguration
{
    public int ContextWindowTokens { get; set; } = 4000;
    public string SystemPrompt { get; set; } = "You are a helpful, professional AI assistant. Provide clear, concise, and accurate responses. Ask for clarification when needed.";
    public bool EnableSafetyFeatures { get; set; } = true;
    public bool EnableConversationMemory { get; set; } = true;
}

public class FailoverConfiguration
{
    public string PrimaryProvider { get; set; } = "OpenAI";
    public string FallbackProvider { get; set; } = "Google";
}

public class ProviderConfiguration
{
    public string Model { get; set; } = string.Empty;
    public int MaxTokens { get; set; } = 1500;
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromMinutes(2);
    public int PermitLimit { get; set; } = 100;
    public int WindowInSeconds { get; set; } = 60;
}

public class OpenAIConfiguration : ProviderConfiguration
{
    public OpenAIConfiguration()
    {
        Model = "gpt-3.5-turbo";
        MaxTokens = 4000;
        PermitLimit = 100;
    }
}

public class GoogleConfiguration : ProviderConfiguration
{
    public GoogleConfiguration()
    {
        Model = "Gemini 2";
        MaxTokens = 8000;
        PermitLimit = 50;
    }
}