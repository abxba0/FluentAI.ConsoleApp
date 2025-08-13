using FluentAI;
using FluentAI.Extensions;
using FluentAI.Abstractions;
using FluentAI.Abstractions.Models;
using FluentAI.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// FluentAI.NET Console Application Demo
// This demonstrates basic usage of the FluentAI.NET library

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("FluentAI.NET Console Application Demo");
        Console.WriteLine("=====================================");
        
        // Initialize the FluentAI client with a placeholder API key
        // In a real application, you would use a valid OpenAI API key
        var apiKey = "your-openai-api-key-here";
        
        try
        {
            // Create a service collection and configure FluentAI
            var services = new ServiceCollection();
            
            // Add logging services
            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
            
            // Add FluentAI services
            services.AddFluentAI()
                .AddOpenAI(options =>
                {
                    options.ApiKey = apiKey;
                    options.Model = "gpt-3.5-turbo";
                })
                .UseDefaultProvider("OpenAI");
            
            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();
            
            Console.WriteLine("FluentAI client initialized successfully!");
            Console.WriteLine($"Using API key: {apiKey.Substring(0, Math.Min(10, apiKey.Length))}...");
            
            // Get the chat model factory
            var chatModelFactory = serviceProvider.GetRequiredService<IChatModelFactory>();
            var chatModel = chatModelFactory.GetModel("OpenAI");
            
            // Demonstrate simple prompt and response
            Console.WriteLine("\nSending a simple prompt to the AI...");
            var prompt = "Hello! Please introduce yourself briefly.";
            Console.WriteLine($"Prompt: {prompt}");
            
            // Create chat messages
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.User, prompt)
            };
            
            // Send prompt and get response
            var requestOptions = new OpenAiRequestOptions();
            var response = await chatModel.GetResponseAsync(messages, requestOptions, CancellationToken.None);
            
            Console.WriteLine($"\nAI Response: {response.Content}");
            Console.WriteLine($"Model ID: {response.ModelId}");
            Console.WriteLine($"Finish Reason: {response.FinishReason}");
            Console.WriteLine($"Token Usage: Input={response.Usage.InputTokens}, Output={response.Usage.OutputTokens}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError occurred: {ex.Message}");
            Console.WriteLine("\nNote: This demo requires a valid OpenAI API key to work properly.");
            Console.WriteLine("Replace 'your-openai-api-key-here' with your actual API key.");
            Console.WriteLine("\nTo get an API key:");
            Console.WriteLine("1. Visit https://platform.openai.com/api-keys");
            Console.WriteLine("2. Create an account or sign in");
            Console.WriteLine("3. Generate a new API key");
            Console.WriteLine("4. Replace the placeholder in this code");
        }
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
