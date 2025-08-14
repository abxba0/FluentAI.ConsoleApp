using Microsoft.Extensions.Configuration;

namespace FluentAI.ConsoleApp.Configuration;

public interface IApiKeyConfigurationService
{
    string GetApiKey(string provider = "OpenAI");
    string GetModel(string provider = "OpenAI");
    ProviderConfiguration GetProviderConfiguration(string provider);
    AiSdkConfiguration GetAiSdkConfiguration();
    string GetDefaultProvider();
    string GetPrimaryProvider();
    string GetFallbackProvider();
}

public class ApiKeyConfigurationService : IApiKeyConfigurationService
{
    private readonly IConfiguration _configuration;

    public ApiKeyConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetApiKey(string provider = "OpenAI")
    {
        // First, try to get from appsettings.json (legacy Aisdk section for backward compatibility)
        var apiKey = _configuration[$"Aisdk:{provider}:ApiKey"];
        
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            return apiKey;
        }

        // Try new structure (though API keys shouldn't be in appsettings for security)
        apiKey = _configuration[$"{provider}:ApiKey"];
        
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            return apiKey;
        }

        // Fallback to environment variables based on provider
        if (provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
        {
            apiKey = _configuration["OPENAI_API_KEY"] ?? _configuration["OpenAI__ApiKey"];
        }
        else if (provider.Equals("Google", StringComparison.OrdinalIgnoreCase))
        {
            apiKey = _configuration["GOOGLE_API_KEY"] ?? _configuration["Google__ApiKey"];
        }
        
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            return apiKey;
        }

        // Return placeholder for demo purposes
        return $"your-{provider.ToLower()}-api-key-here";
    }

    public string GetModel(string provider = "OpenAI")
    {
        // Try new structure first
        var model = _configuration[$"{provider}:Model"];
        
        if (!string.IsNullOrWhiteSpace(model))
        {
            return model;
        }

        // Fallback to legacy structure for backward compatibility
        model = _configuration[$"Aisdk:{provider}:Model"];
        
        if (!string.IsNullOrWhiteSpace(model))
        {
            return model;
        }

        // Provider-specific defaults
        return provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase) ? "gpt-3.5-turbo" : "Gemini 2";
    }

    public ProviderConfiguration GetProviderConfiguration(string provider)
    {
        ProviderConfiguration config;
        
        if (provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
        {
            config = new OpenAIConfiguration();
        }
        else if (provider.Equals("Google", StringComparison.OrdinalIgnoreCase))
        {
            config = new GoogleConfiguration();
        }
        else
        {
            config = new ProviderConfiguration();
        }

        _configuration.GetSection(provider).Bind(config);
        return config;
    }

    public AiSdkConfiguration GetAiSdkConfiguration()
    {
        var config = new AiSdkConfiguration();
        _configuration.GetSection("AiSdk").Bind(config);
        return config;
    }

    public string GetDefaultProvider()
    {
        return _configuration["AiSdk:DefaultProvider"] ?? "OpenAI";
    }

    public string GetPrimaryProvider()
    {
        return _configuration["AiSdk:Failover:PrimaryProvider"] ?? "OpenAI";
    }

    public string GetFallbackProvider()
    {
        return _configuration["AiSdk:Failover:FallbackProvider"] ?? "Google";
    }
}