using FluentAI.Abstractions.Models;

namespace FluentAI.ConsoleApp.Services;

public interface IConversationManager
{
    void AddMessage(ChatMessage message);
    void AddMessage(ChatRole role, string content);
    List<ChatMessage> GetMessages();
    List<ChatMessage> GetMessagesWithSummary(int maxTokens);
    void ClearConversation();
    void RemoveLastUserMessage();
    int EstimateTokenCount();
    bool IsNearTokenLimit(int maxTokens);
}

public class ConversationManager : IConversationManager
{
    private readonly List<ChatMessage> _messages = new();
    private readonly List<string> _summaries = new();
    private const int TOKENS_PER_CHAR = 4; // Rough estimate: 1 token ≈ 4 characters
    private const double TOKEN_LIMIT_THRESHOLD = 0.8; // Start summarizing at 80% of limit

    public void AddMessage(ChatMessage message)
    {
        _messages.Add(message);
    }

    public void AddMessage(ChatRole role, string content)
    {
        _messages.Add(new ChatMessage(role, content));
    }

    public List<ChatMessage> GetMessages()
    {
        return new List<ChatMessage>(_messages);
    }

    public List<ChatMessage> GetMessagesWithSummary(int maxTokens)
    {
        var allMessages = new List<ChatMessage>();
        
        // Add summaries as system messages
        foreach (var summary in _summaries)
        {
            allMessages.Add(new ChatMessage(ChatRole.System, $"Previous conversation summary: {summary}"));
        }
        
        // Add recent messages that fit within token limit
        var recentMessages = GetRecentMessagesThatFit(maxTokens);
        allMessages.AddRange(recentMessages);
        
        return allMessages;
    }

    public void ClearConversation()
    {
        _messages.Clear();
        _summaries.Clear();
    }

    public void RemoveLastUserMessage()
    {
        // Find and remove the last user message
        for (int i = _messages.Count - 1; i >= 0; i--)
        {
            if (_messages[i].Role == ChatRole.User)
            {
                _messages.RemoveAt(i);
                break;
            }
        }
    }

    public int EstimateTokenCount()
    {
        var totalChars = _messages.Sum(m => m.Content?.Length ?? 0);
        return totalChars / TOKENS_PER_CHAR;
    }

    public bool IsNearTokenLimit(int maxTokens)
    {
        return EstimateTokenCount() > (maxTokens * TOKEN_LIMIT_THRESHOLD);
    }

    private List<ChatMessage> GetRecentMessagesThatFit(int maxTokens)
    {
        var result = new List<ChatMessage>();
        var currentTokens = 0;
        
        // Reserve space for summaries
        var summaryTokens = _summaries.Sum(s => s.Length / TOKENS_PER_CHAR);
        var availableTokens = maxTokens - summaryTokens - 500; // Reserve 500 for response
        
        // Add messages from most recent backwards until we hit the limit
        for (int i = _messages.Count - 1; i >= 0; i--)
        {
            var messageTokens = (_messages[i].Content?.Length ?? 0) / TOKENS_PER_CHAR;
            if (currentTokens + messageTokens > availableTokens)
            {
                // If we can't fit this message, summarize older messages
                if (i > 0)
                {
                    SummarizeOlderMessages(i);
                }
                break;
            }
            
            result.Insert(0, _messages[i]);
            currentTokens += messageTokens;
        }
        
        return result;
    }

    private void SummarizeOlderMessages(int endIndex)
    {
        if (endIndex <= 0) return;
        
        var messagesToSummarize = _messages.Take(endIndex).ToList();
        if (messagesToSummarize.Count == 0) return;
        
        // Create a simple summary of key points
        var userMessages = messagesToSummarize.Where(m => m.Role == ChatRole.User).ToList();
        var assistantMessages = messagesToSummarize.Where(m => m.Role == ChatRole.Assistant).ToList();
        
        var summary = $"User discussed: {string.Join(", ", userMessages.Take(3).Select(m => TruncateContent(m.Content, 50)))}";
        if (userMessages.Count > 3)
        {
            summary += $" and {userMessages.Count - 3} other topics";
        }
        
        if (assistantMessages.Any())
        {
            summary += $". Assistant provided guidance on these topics.";
        }
        
        _summaries.Add(summary);
        
        // Remove the summarized messages but keep recent ones
        _messages.RemoveRange(0, endIndex);
    }

    private static string TruncateContent(string? content, int maxLength)
    {
        if (string.IsNullOrEmpty(content)) return "";
        return content.Length <= maxLength ? content : content.Substring(0, maxLength) + "...";
    }
}