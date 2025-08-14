using FluentAI;
using FluentAI.ConsoleApp.Configuration;
using FluentAI.Extensions;

public static class FluentAiBuilderExtensions
{
    public static IFluentAiBuilder AddGemini(this IFluentAiBuilder builder, Action<ProviderConfiguration> configure)
    {
        var config = new ProviderConfiguration();
        configure(config);

        // Add Gemini provider logic here
        // Example: builder.AddProvider("Gemini", config);

        return builder;
       }
   }