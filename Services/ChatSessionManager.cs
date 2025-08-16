namespace FluentAI.ConsoleApp.Services;

public interface IChatSessionManager
{
    bool IsCommand(string input);
    ChatCommand ParseCommand(string input);
    void HandleCommand(ChatCommand command, IConversationManager conversationManager);
    string GetHelpText();
}

public enum ChatCommandType
{
    New,
    Delete,
    Help,
    Quit,
    Unknown
}

public record ChatCommand(ChatCommandType Type, string OriginalInput);

public class ChatSessionManager : IChatSessionManager
{
    public bool IsCommand(string input)
    {
        return !string.IsNullOrWhiteSpace(input) && input.TrimStart().StartsWith(':');
    }

    public ChatCommand ParseCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new ChatCommand(ChatCommandType.Unknown, input);

        var command = input.Trim().ToLowerInvariant();
        
        return command switch
        {
            ":new" => new ChatCommand(ChatCommandType.New, input),
            ":del" => new ChatCommand(ChatCommandType.Delete, input),
            ":help" => new ChatCommand(ChatCommandType.Help, input),
            ":quit" or ":exit" or ":q" => new ChatCommand(ChatCommandType.Quit, input),
            _ => new ChatCommand(ChatCommandType.Unknown, input)
        };
    }

    public void HandleCommand(ChatCommand command, IConversationManager conversationManager)
    {
        switch (command.Type)
        {
            case ChatCommandType.New:
                conversationManager.ClearConversation();
                Console.WriteLine("✓ Started new conversation. Previous history cleared.");
                break;
                
            case ChatCommandType.Delete:
                conversationManager.RemoveLastUserMessage();
                Console.WriteLine("✓ Removed your last message.");
                break;
                
            case ChatCommandType.Help:
                Console.WriteLine(GetHelpText());
                break;
                
            case ChatCommandType.Quit:
                Console.WriteLine("Goodbye!");
                Environment.Exit(0);
                break;
                
            case ChatCommandType.Unknown:
                Console.WriteLine($"Unknown command: {command.OriginalInput}");
                Console.WriteLine("Type :help for available commands.");
                break;
        }
    }

    public string GetHelpText()
    {
        return """
        Available Commands:
        ==================
        :new    - Start a new conversation (clears history)
        :del    - Delete your last message
        :help   - Show this help message
        :quit   - Exit the application (:q, :exit also work)
        
        Simply type your message and press Enter to chat with the AI.
        Commands are vim-style and start with a colon (:).
        """;
    }
}