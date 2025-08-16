using System.Text.RegularExpressions;

namespace FluentAI.ConsoleApp.Services;

public interface IInputValidationService
{
    ValidationResult ValidateInput(string input);
    string SanitizeInput(string input);
    bool ContainsRiskyContent(string input);
    string GetClarificationPrompt(string input);
}

public record ValidationResult(
    bool IsValid, 
    string? ErrorMessage = null, 
    bool NeedsClarification = false,
    string? ClarificationPrompt = null);

public class InputValidationService : IInputValidationService
{
    private static readonly string[] RiskyKeywords = 
    {
        "hack", "exploit", "malware", "virus", "illegal", "harmful", "dangerous",
        "suicide", "self-harm", "violence", "bomb", "weapon", "drug", "abuse"
    };
    
    private static readonly string[] ProfanityWords = 
    {
        // Add common profanity words here - keeping minimal for demo
        "damn", "hell"
    };

    public ValidationResult ValidateInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new ValidationResult(false, "Input cannot be empty.");
        }

        if (input.Length > 4000) // Reasonable limit for input
        {
            return new ValidationResult(false, "Input is too long. Please keep messages under 4000 characters.");
        }

        // Check for risky content
        if (ContainsRiskyContent(input))
        {
            return new ValidationResult(false, 
                "Your message contains content that cannot be processed. Please rephrase your request in a safe and appropriate manner.");
        }

        // Check if input is too vague and needs clarification
        if (IsVagueInput(input))
        {
            var clarificationPrompt = GetClarificationPrompt(input);
            return new ValidationResult(true, null, true, clarificationPrompt);
        }

        return new ValidationResult(true);
    }

    public string SanitizeInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove excessive whitespace
        input = Regex.Replace(input, @"\s+", " ").Trim();
        
        // Remove potential code injection patterns
        input = Regex.Replace(input, @"<script.*?</script>", "", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"javascript:", "", RegexOptions.IgnoreCase);
        
        // Limit consecutive special characters
        input = Regex.Replace(input, @"[!@#$%^&*()]{3,}", "");
        
        return input;
    }

    public bool ContainsRiskyContent(string input)
    {
        var lowerInput = input.ToLowerInvariant();
        
        // Check for risky keywords
        if (RiskyKeywords.Any(keyword => lowerInput.Contains(keyword)))
        {
            return true;
        }
        
        // Check for excessive profanity
        var profanityCount = ProfanityWords.Count(word => lowerInput.Contains(word));
        if (profanityCount > 2) // Allow some flexibility
        {
            return true;
        }
        
        // Check for potential prompt injection
        if (lowerInput.Contains("ignore previous instructions") || 
            lowerInput.Contains("forget your role") ||
            lowerInput.Contains("act as if you are"))
        {
            return true;
        }
        
        return false;
    }

    public string GetClarificationPrompt(string input)
    {
        var lowerInput = input.ToLowerInvariant();
        
        if (lowerInput.Length < 10)
        {
            return "Could you provide more details about what you're looking for?";
        }
        
        if (lowerInput.Contains("help") && lowerInput.Split(' ').Length < 3)
        {
            return "I'd be happy to help! Could you tell me specifically what you need assistance with?";
        }
        
        if (lowerInput.Contains("how") && lowerInput.Split(' ').Length < 4)
        {
            return "Could you be more specific about what process or topic you'd like to learn about?";
        }
        
        if (lowerInput.Contains("what") && lowerInput.Split(' ').Length < 4)
        {
            return "Could you provide more context about what specific information you're seeking?";
        }
        
        return "Could you elaborate on your question or provide more context so I can give you a better response?";
    }
    
    private static bool IsVagueInput(string input)
    {
        var lowerInput = input.ToLowerInvariant();
        var words = lowerInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        // Very short inputs are often vague
        if (words.Length < 3)
        {
            var vaguePhrases = new[] { "help", "what", "how", "tell me", "explain", "hi", "hello" };
            return vaguePhrases.Any(phrase => lowerInput.Contains(phrase));
        }
        
        return false;
    }
}