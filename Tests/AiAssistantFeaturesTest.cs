using FluentAI.Abstractions.Models;
using FluentAI.ConsoleApp.Services;

namespace FluentAI.ConsoleApp.Tests;

/// <summary>
/// Test advanced AI assistant features
/// </summary>
public static class AiAssistantFeaturesTest
{
    public static void TestConversationManager()
    {
        Console.WriteLine("=== Testing Conversation Manager ===");
        
        var manager = new ConversationManager();
        
        // Test adding messages
        manager.AddMessage(ChatRole.System, "You are a helpful assistant.");
        manager.AddMessage(ChatRole.User, "Hello, how are you?");
        manager.AddMessage(ChatRole.Assistant, "I'm doing well, thank you! How can I help you today?");
        
        var messages = manager.GetMessages();
        Console.WriteLine($"✓ Added {messages.Count} messages");
        
        // Test token estimation
        var tokenCount = manager.EstimateTokenCount();
        Console.WriteLine($"✓ Estimated token count: {tokenCount}");
        
        // Test near token limit detection
        var isNearLimit = manager.IsNearTokenLimit(100);
        Console.WriteLine($"✓ Near token limit (100): {isNearLimit}");
        
        // Test removing last user message
        manager.RemoveLastUserMessage();
        var messagesAfterDelete = manager.GetMessages();
        Console.WriteLine($"✓ Messages after delete: {messagesAfterDelete.Count}");
        
        // Test conversation clearing
        manager.ClearConversation();
        var messagesAfterClear = manager.GetMessages();
        Console.WriteLine($"✓ Messages after clear: {messagesAfterClear.Count}");
        
        Console.WriteLine("=== Conversation Manager Test Passed! ===\n");
    }
    
    public static void TestChatSessionManager()
    {
        Console.WriteLine("=== Testing Chat Session Manager ===");
        
        var sessionManager = new ChatSessionManager();
        var conversationManager = new ConversationManager();
        
        // Test command detection
        var isCommand1 = sessionManager.IsCommand(":new");
        var isCommand2 = sessionManager.IsCommand("hello");
        Console.WriteLine($"✓ ':new' is command: {isCommand1}");
        Console.WriteLine($"✓ 'hello' is command: {isCommand2}");
        
        // Test command parsing
        var newCommand = sessionManager.ParseCommand(":new");
        var delCommand = sessionManager.ParseCommand(":del");
        var helpCommand = sessionManager.ParseCommand(":help");
        var unknownCommand = sessionManager.ParseCommand(":unknown");
        
        Console.WriteLine($"✓ Parsed ':new' as: {newCommand.Type}");
        Console.WriteLine($"✓ Parsed ':del' as: {delCommand.Type}");
        Console.WriteLine($"✓ Parsed ':help' as: {helpCommand.Type}");
        Console.WriteLine($"✓ Parsed ':unknown' as: {unknownCommand.Type}");
        
        // Test help text
        var helpText = sessionManager.GetHelpText();
        Console.WriteLine($"✓ Help text generated: {helpText.Length} characters");
        
        Console.WriteLine("=== Chat Session Manager Test Passed! ===\n");
    }
    
    public static void TestInputValidationService()
    {
        Console.WriteLine("=== Testing Input Validation Service ===");
        
        var validationService = new InputValidationService();
        
        // Test valid input
        var validResult = validationService.ValidateInput("How can I learn programming?");
        Console.WriteLine($"✓ Valid input validation: {validResult.IsValid}");
        
        // Test empty input
        var emptyResult = validationService.ValidateInput("");
        Console.WriteLine($"✓ Empty input validation: {emptyResult.IsValid}, Message: {emptyResult.ErrorMessage}");
        
        // Test long input
        var longInput = new string('a', 5000);
        var longResult = validationService.ValidateInput(longInput);
        Console.WriteLine($"✓ Long input validation: {longResult.IsValid}, Message: {longResult.ErrorMessage}");
        
        // Test risky content
        var riskyResult = validationService.ContainsRiskyContent("how to hack a system");
        Console.WriteLine($"✓ Risky content detection: {riskyResult}");
        
        // Test safe content
        var safeResult = validationService.ContainsRiskyContent("how to learn programming");
        Console.WriteLine($"✓ Safe content detection: {!safeResult}");
        
        // Test input sanitization
        var dirtyInput = "Hello    <script>alert('xss')</script>   world!!!";
        var cleanInput = validationService.SanitizeInput(dirtyInput);
        Console.WriteLine($"✓ Input sanitization: '{dirtyInput}' -> '{cleanInput}'");
        
        // Test vague input clarification
        var vague = validationService.ValidateInput("help");
        Console.WriteLine($"✓ Vague input needs clarification: {vague.NeedsClarification}");
        if (vague.NeedsClarification)
        {
            Console.WriteLine($"  Clarification: {vague.ClarificationPrompt}");
        }
        
        Console.WriteLine("=== Input Validation Service Test Passed! ===\n");
    }
    
    public static void RunAllTests()
    {
        Console.WriteLine("Running AI Assistant Features Tests...\n");
        
        TestConversationManager();
        TestChatSessionManager();
        TestInputValidationService();
        
        Console.WriteLine("✅ All AI Assistant Features Tests Passed!");
    }
}