using FluentAI;
using FluentAI.Abstractions;
using FluentAI.Abstractions.Models;
using FluentAI.Configuration;
using FluentAI.Extensions;
using FluentAI.ConsoleApp.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FluentAI.ConsoleApp.Examples;

/// <summary>
/// Example demonstrating how to use HuggingFace models with FluentAI.NET
/// </summary>
public static class HuggingFaceExample
{
    /// <summary>
    /// Demonstrates basic text generation using a HuggingFace model
    /// </summary>
    /// <param name="apiKey">Your HuggingFace API key</param>
    /// <param name="modelUrl">HuggingFace model URL (e.g., https://api-inference.huggingface.co/models/microsoft/DialoGPT-medium)</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public static async Task RunTextGenerationExample(string apiKey, string modelUrl)
    {
        Console.WriteLine("=== HuggingFace Text Generation Example ===");
        
        // Setup services
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        
        // Configure FluentAI with HuggingFace provider
        services.AddFluentAI()
            .AddHuggingFace(options =>
            {
                options.ApiKey = apiKey;
                options.ModelId = modelUrl;
                options.RequestTimeout = TimeSpan.FromMinutes(2);
            })
            .UseDefaultProvider("HuggingFace");
        
        var serviceProvider = services.BuildServiceProvider();
        
        try
        {
            // Get the chat model
            var chatModelFactory = serviceProvider.GetRequiredService<IChatModelFactory>();
            var chatModel = chatModelFactory.GetModel("HuggingFace");
            
            // Create a conversation
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.User, "Hello! Can you tell me a short story about a robot?")
            };
            
            // Send the prompt
            Console.WriteLine("Sending prompt to HuggingFace model...");
            Console.WriteLine($"Model: {modelUrl}");
            Console.WriteLine($"Prompt: {messages[0].Content}");
            Console.WriteLine();
            
            var requestOptions = new HuggingFaceRequestOptions();
            var response = await chatModel.GetResponseAsync(messages, requestOptions, CancellationToken.None);
            
            // Display results
            Console.WriteLine("Response from HuggingFace:");
            Console.WriteLine($"Content: {response.Content}");
            Console.WriteLine($"Model ID: {response.ModelId}");
            Console.WriteLine($"Finish Reason: {response.FinishReason}");
            Console.WriteLine($"Token Usage: Input={response.Usage.InputTokens}, Output={response.Usage.OutputTokens}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("Note: Make sure you have a valid HuggingFace API key and the model URL is correct.");
        }
    }
    
    /// <summary>
    /// Example showing how to configure HuggingFace with different models
    /// </summary>
    public static void ShowConfigurationExamples()
    {
        Console.WriteLine("=== HuggingFace Configuration Examples ===");
        Console.WriteLine();
        
        Console.WriteLine("1. Popular HuggingFace Model URLs:");
        Console.WriteLine("   • Conversational AI: https://api-inference.huggingface.co/models/microsoft/DialoGPT-medium");
        Console.WriteLine("   • Text Generation: https://api-inference.huggingface.co/models/gpt2");
        Console.WriteLine("   • Question Answering: https://api-inference.huggingface.co/models/deepset/roberta-base-squad2");
        Console.WriteLine("   • Summarization: https://api-inference.huggingface.co/models/facebook/bart-large-cnn");
        Console.WriteLine();
        
        Console.WriteLine("2. appsettings.json configuration:");
        Console.WriteLine(@"{
  ""AiSdk"": {
    ""DefaultProvider"": ""HuggingFace"",
    ""Failover"": {
      ""PrimaryProvider"": ""HuggingFace"",
      ""FallbackProvider"": ""OpenAI""
    }
  },
  ""HuggingFace"": {
    ""Model"": ""https://api-inference.huggingface.co/models/microsoft/DialoGPT-medium"",
    ""MaxTokens"": 1000,
    ""RequestTimeout"": ""00:02:00"",
    ""PermitLimit"": 60,
    ""WindowInSeconds"": 60
  }
}");
        Console.WriteLine();
        
        Console.WriteLine("3. Environment variable configuration:");
        Console.WriteLine("   export HUGGINGFACE_API_KEY=\"your-huggingface-api-key-here\"");
        Console.WriteLine("   # or");
        Console.WriteLine("   export HuggingFace:ApiKey=\"your-huggingface-api-key-here\"");
        Console.WriteLine();
        
        Console.WriteLine("4. User secrets configuration:");
        Console.WriteLine("   dotnet user-secrets set \"HUGGINGFACE_API_KEY\" \"your-huggingface-api-key-here\"");
        Console.WriteLine("   # or");
        Console.WriteLine("   dotnet user-secrets set \"HuggingFace:ApiKey\" \"your-huggingface-api-key-here\"");
    }
}