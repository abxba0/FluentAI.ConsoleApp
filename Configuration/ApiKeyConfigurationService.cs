using Microsoft.Extensions.Configuration;

namespace FluentAI.ConsoleApp.Configuration;

public interface IApiKeyConfigurationService
{
    string GetApiKey();
    string GetModel();
}

public class ApiKeyConfigurationService : IApiKeyConfigurationService
{
    private readonly IConfiguration _configuration;

    public ApiKeyConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetApiKey()
    {
        // First, try to get from appsettings.json Aisdk section
        var apiKey = _configuration["Aisdk:OpenAI:ApiKey"];
        
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            return apiKey;
        }

        // Fallback to environment variables
        apiKey = _configuration["OPENAI_API_KEY"];
        
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            return apiKey;
        }

        // Another common environment variable name
        apiKey = _configuration["OpenAI__ApiKey"];
        
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            return apiKey;
        }

        // Return placeholder for demo purposes
        return "your-openai-api-key-here";
    }

    public string GetModel()
    {
        return _configuration["Aisdk:OpenAI:Model"] ?? "gpt-3.5-turbo";
    }
}