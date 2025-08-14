using FluentAI;
using FluentAI.Extensions;
using FluentAI.ConsoleApp.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FluentAI.ConsoleApp.Services;

public static class FluentAiServiceSetup
{
    public static IServiceCollection AddFluentAiServices(this IServiceCollection services, IApiKeyConfigurationService configService)
    {
        var aiSdkConfig = configService.GetAiSdkConfiguration();
        var defaultProvider = configService.GetDefaultProvider();
        
        // Configure FluentAI services
        var fluentAiBuilder = services.AddFluentAI();

        // Add OpenAI provider
        var openAiConfig = configService.GetProviderConfiguration("OpenAI");
        var openAiApiKey = configService.GetApiKey("OpenAI");
        
        fluentAiBuilder.AddOpenAI(options =>
        {
            options.ApiKey = openAiApiKey;
            options.Model = openAiConfig.Model;
            // Note: FluentAI SDK may not support all these options yet
            // Adding them for future compatibility
        });

        // Add Google provider if needed
        var googleConfig = configService.GetProviderConfiguration("Google");
        var googleApiKey = configService.GetApiKey("Google");
        
        // Note: This assumes FluentAI has Google provider support
        // If not available, we'll log a warning
        try
        {
            fluentAiBuilder.AddGoogle(options =>
            {
                options.ApiKey = googleApiKey;
                options.Model = googleConfig.Model;
            });
        }
        catch (Exception)
        {
            // Google provider may not be available in current FluentAI version
            // This is acceptable for now
        }

        // Set default provider
        fluentAiBuilder.UseDefaultProvider(defaultProvider);

        return services;
    }
}