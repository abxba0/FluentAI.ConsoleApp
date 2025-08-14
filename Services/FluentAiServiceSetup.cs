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
        var apiKey = configService.GetApiKey();
        var model = configService.GetModel();

        // Add FluentAI services
        services.AddFluentAI()
            .AddOpenAI(options =>
            {
                options.ApiKey = apiKey;
                options.Model = model;
            })
            .UseDefaultProvider("OpenAI");

        return services;
    }
}