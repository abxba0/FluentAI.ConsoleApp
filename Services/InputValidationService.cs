using System.Text.RegularExpressions;
using FluentAI.Abstractions.Security;
using Microsoft.Extensions.Logging;

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

/// <summary>
/// Input validation service that uses fluentai-dotnet's security implementations.
/// Acts as an adapter between IInputValidationService and IInputSanitizer.
/// </summary>
public class InputValidationService : IInputValidationService
{
    private readonly IInputSanitizer _inputSanitizer;
    private readonly ILogger<InputValidationService> _logger;

    public InputValidationService(IInputSanitizer inputSanitizer, ILogger<InputValidationService> logger)
    {
        _inputSanitizer = inputSanitizer ?? throw new ArgumentNullException(nameof(inputSanitizer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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

        // Use fluentai-dotnet's advanced risk assessment
        var riskAssessment = _inputSanitizer.AssessRisk(input);
        
        if (riskAssessment.ShouldBlock)
        {
            var concernDetails = riskAssessment.DetectedConcerns.Any() 
                ? $" Detected concerns: {string.Join(", ", riskAssessment.DetectedConcerns)}"
                : "";
            
            _logger.LogWarning("High-risk content blocked. Risk level: {RiskLevel}{Concerns}", 
                riskAssessment.RiskLevel, concernDetails);
                
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

        // Use fluentai-dotnet's advanced sanitization
        return _inputSanitizer.SanitizeContent(input);
    }

    public bool ContainsRiskyContent(string input)
    {
        // Use fluentai-dotnet's advanced safety checking
        return !_inputSanitizer.IsContentSafe(input);
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