using FluentAI.ConsoleApp.Services;

namespace FluentAI.ConsoleApp.Tests;

/// <summary>
/// Demo script to show the interactive chat interface features
/// </summary>
public static class InteractiveChatDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("=== Interactive Chat Interface Demo ===\n");
        
        var conversationManager = new ConversationManager();
        var sessionManager = new ChatSessionManager();
        var validationService = new InputValidationService();
        
        // Simulate the welcome screen
        Console.WriteLine("FluentAI.NET Advanced AI Assistant");
        Console.WriteLine("==================================");
        Console.WriteLine("Provider: OpenAI");
        Console.WriteLine("Model: gpt-4");
        Console.WriteLine("Context Window: 4000 tokens");
        Console.WriteLine("Safety Features: Enabled");
        Console.WriteLine("Conversation Memory: Enabled");
        
        Console.WriteLine("\n" + sessionManager.GetHelpText());
        Console.WriteLine("Chat started! Type your message or use commands starting with ':'");
        Console.WriteLine("=====================================\n");
        
        // Demo conversation flow
        DemoUserInput("Hello! How are you today?", conversationManager, sessionManager, validationService);
        DemoAiResponse("Hello! I'm doing well, thank you for asking. I'm here to help you with any questions or tasks you might have. How can I assist you today?", conversationManager);
        
        DemoUserInput("Can you help me learn Python?", conversationManager, sessionManager, validationService);
        DemoAiResponse("Absolutely! I'd be happy to help you learn Python. Python is a great programming language for beginners. What specific aspect would you like to start with - basic syntax, data types, functions, or perhaps a particular project?", conversationManager);
        
        // Demo commands
        Console.WriteLine("\n--- Demonstrating Commands ---");
        DemoCommand(":help", sessionManager, conversationManager);
        DemoCommand(":del", sessionManager, conversationManager);
        DemoCommand(":new", sessionManager, conversationManager);
        
        // Demo safety features
        Console.WriteLine("\n--- Demonstrating Safety Features ---");
        DemoUserInput("hack", conversationManager, sessionManager, validationService);
        DemoUserInput("help", conversationManager, sessionManager, validationService);
        
        // Show token management
        Console.WriteLine("\n--- Token Management Info ---");
        Console.WriteLine($"📊 Current conversation tokens: {conversationManager.EstimateTokenCount()}");
        Console.WriteLine($"📊 Near token limit (100): {conversationManager.IsNearTokenLimit(100)}");
        
        Console.WriteLine("\n=== Demo Complete ===");
    }
    
    private static void DemoUserInput(string input, IConversationManager conversation, IChatSessionManager session, IInputValidationService validation)
    {
        Console.WriteLine($"\nYou: {input}");
        
        if (session.IsCommand(input))
        {
            var command = session.ParseCommand(input);
            session.HandleCommand(command, conversation);
            return;
        }
        
        var validationResult = validation.ValidateInput(input);
        if (!validationResult.IsValid)
        {
            Console.WriteLine($"⚠️  {validationResult.ErrorMessage}");
            return;
        }
        
        if (validationResult.NeedsClarification)
        {
            Console.WriteLine($"🤔 {validationResult.ClarificationPrompt}");
            return;
        }
        
        var sanitized = validation.SanitizeInput(input);
        conversation.AddMessage(FluentAI.Abstractions.Models.ChatRole.User, sanitized);
    }
    
    private static void DemoAiResponse(string response, IConversationManager conversation)
    {
        Console.WriteLine($"\nAI: {response}");
        conversation.AddMessage(FluentAI.Abstractions.Models.ChatRole.Assistant, response);
        Console.WriteLine($"📊 Tokens: Input=15, Output=25, Total={conversation.EstimateTokenCount()}");
    }
    
    private static void DemoCommand(string command, IChatSessionManager session, IConversationManager conversation)
    {
        Console.WriteLine($"\nYou: {command}");
        var parsedCommand = session.ParseCommand(command);
        session.HandleCommand(parsedCommand, conversation);
    }
}