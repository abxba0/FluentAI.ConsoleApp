using FluentAI.Abstractions;
using FluentAI.Abstractions.Models;
using FluentAI.Configuration;
using FluentAI.ConsoleApp.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentAI.ConsoleApp.Services;

public interface IInteractiveChatService
{
    Task StartChatAsync();
}

public class InteractiveChatService : IInteractiveChatService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IApiKeyConfigurationService _configService;
    private readonly IConversationManager _conversationManager;
    private readonly IChatSessionManager _sessionManager;
    private readonly IInputValidationService _validationService;

    public InteractiveChatService(
        IServiceProvider serviceProvider,
        IApiKeyConfigurationService configService,
        IConversationManager conversationManager,
        IChatSessionManager sessionManager,
        IInputValidationService validationService)
    {
        _serviceProvider = serviceProvider;
        _configService = configService;
        _conversationManager = conversationManager;
        _sessionManager = sessionManager;
        _validationService = validationService;
    }

    public async Task StartChatAsync()
    {
        DisplayWelcomeMessage();
        
        var chatModel = await InitializeChatModel();
        if (chatModel == null)
        {
            Console.WriteLine("Failed to initialize AI chat model. Exiting...");
            return;
        }

        var aiSdkConfig = _configService.GetAiSdkConfiguration();
        var systemPrompt = aiSdkConfig.Chat.SystemPrompt;
        
        // Add system message to conversation
        _conversationManager.AddMessage(ChatRole.System, systemPrompt);
        
        Console.WriteLine("\n" + _sessionManager.GetHelpText());
        Console.WriteLine("\nChat started! Type your message or use commands starting with ':'");
        Console.WriteLine("=====================================\n");

        while (true)
        {
            await ProcessUserInput(chatModel, aiSdkConfig);
        }
    }

    private Task DisplayWelcomeMessage()
    {
        Console.Clear();
        Console.WriteLine("FluentAI.NET Advanced AI Assistant");
        Console.WriteLine("==================================");
        
        var config = _configService.GetAiSdkConfiguration();
        var provider = _configService.GetDefaultProvider();
        var model = _configService.GetModel(provider);
        
        Console.WriteLine($"Provider: {provider}");
        Console.WriteLine($"Model: {model}");
        Console.WriteLine($"Context Window: {config.Chat.ContextWindowTokens} tokens");
        Console.WriteLine($"Safety Features: {(config.Chat.EnableSafetyFeatures ? "Enabled" : "Disabled")}");
        Console.WriteLine($"Conversation Memory: {(config.Chat.EnableConversationMemory ? "Enabled" : "Disabled")}");
        
        return Task.CompletedTask;
    }

    private async Task<IChatModel?> InitializeChatModel()
    {
        try
        {
            var defaultProvider = _configService.GetDefaultProvider();
            var primaryProvider = _configService.GetPrimaryProvider();
            var fallbackProvider = _configService.GetFallbackProvider();

            // Try primary provider first
            var chatModel = await TryGetChatModel(primaryProvider);
            if (chatModel != null) return chatModel;

            // Try fallback provider
            if (!primaryProvider.Equals(fallbackProvider, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Primary provider failed, trying fallback: {fallbackProvider}");
                chatModel = await TryGetChatModel(fallbackProvider);
                if (chatModel != null) return chatModel;
            }

            Console.WriteLine("All providers failed. Please check your API keys and configuration.");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing chat model: {ex.Message}");
            return null;
        }
    }

    private async Task<IChatModel?> TryGetChatModel(string provider)
    {
        try
        {
            var chatModelFactory = _serviceProvider.GetRequiredService<IChatModelFactory>();
            var chatModel = chatModelFactory.GetModel(provider);
            
            // Test with a simple message
            var testMessages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.User, "Hello")
            };
            
            var requestOptions = new OpenAiRequestOptions();
            await chatModel.GetResponseAsync(testMessages, requestOptions, CancellationToken.None);
            
            Console.WriteLine($"✓ Successfully connected to {provider}");
            return chatModel;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Failed to connect to {provider}: {ex.Message}");
            return null;
        }
    }

    private async Task ProcessUserInput(IChatModel chatModel, AiSdkConfiguration config)
    {
        Console.Write("\nYou: ");
        var input = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(input))
            return;

        // Handle commands
        if (_sessionManager.IsCommand(input))
        {
            var command = _sessionManager.ParseCommand(input);
            _sessionManager.HandleCommand(command, _conversationManager);
            return;
        }

        // Validate and sanitize input
        if (config.Chat.EnableSafetyFeatures)
        {
            var validation = _validationService.ValidateInput(input);
            if (!validation.IsValid)
            {
                Console.WriteLine($"⚠️  {validation.ErrorMessage}");
                return;
            }

            if (validation.NeedsClarification)
            {
                Console.WriteLine($"🤔 {validation.ClarificationPrompt}");
                return;
            }

            input = _validationService.SanitizeInput(input);
        }

        // Add user message to conversation
        _conversationManager.AddMessage(ChatRole.User, input);

        try
        {
            // Get messages with summarization if needed
            var messages = _conversationManager.GetMessagesWithSummary(config.Chat.ContextWindowTokens);
            
            // Show token usage warning if near limit
            if (_conversationManager.IsNearTokenLimit(config.Chat.ContextWindowTokens))
            {
                Console.WriteLine("ℹ️  Approaching token limit. Older messages will be summarized.");
            }

            Console.Write("\nAI: ");
            
            // Get AI response
            var requestOptions = new OpenAiRequestOptions();
            var response = await chatModel.GetResponseAsync(messages, requestOptions, CancellationToken.None);
            
            Console.WriteLine(response.Content);
            
            // Add AI response to conversation
            _conversationManager.AddMessage(ChatRole.Assistant, response.Content ?? "");
            
            // Show token usage info
            Console.WriteLine($"\n📊 Tokens: Input={response.Usage.InputTokens}, Output={response.Usage.OutputTokens}, Total={_conversationManager.EstimateTokenCount()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error getting AI response: {ex.Message}");
            Console.WriteLine("Please try again or check your connection.");
        }
    }
}