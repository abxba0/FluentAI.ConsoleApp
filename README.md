# FluentAI.NET Console Application

# FluentAI.NET Advanced AI Assistant Console Application

A C# .NET console application demonstrating advanced AI assistant capabilities using the FluentAI.NET NuGet package (version 1.0.5) with interactive chat, conversation memory, safety features, and intelligent session management.

## Advanced Features

### 🧠 Conversation Memory
- Maintains an in-memory record of the ongoing conversation during a session
- Automatically resets memory when the user starts a new chat (`:new` command)
- Allows users to delete their last input with `:del` command

### 🎯 Token Awareness & Summarization
- Tracks token usage and estimates conversation length
- Automatically summarizes older dialogue when approaching the customizable context window limit
- Summaries preserve user goals, key facts, and unresolved questions while keeping recent messages in full

### 🔗 Context Awareness
- Integrates both recent dialogue and stored summaries to maintain continuity and relevance
- Provides seamless conversation flow with intelligent context management

### ✨ Response Quality
- Ensures all answers are clear, concise, professional, and helpful
- Maintains a polite, friendly tone unless otherwise instructed
- Prompts for clarification when user input is ambiguous

### 🛡️ AI Safety & Quality Features
- **Input Validation**: Validates user inputs for clarity and appropriateness
- **Content Sanitization**: Prevents unsafe, harmful, or inappropriate content processing
- **Risk Assessment**: Flags and filters risky or sensitive content
- **Prompt Injection Protection**: Guards against malicious prompt manipulation

### 🎮 Interactive Chat Session Management
- **Vim-style Commands**: Intuitive command interface starting with `:`
- **`:new`** - Start a new conversation (clears history)
- **`:del`** - Delete your last message
- **`:help`** - Show available commands
- **`:quit`** - Exit the application (`:q`, `:exit` also work)

### ⚡ Core Features
- Flexible API key configuration with multiple fallback options
- Configuration via `appsettings.json`, environment variables, and `dotnet user-secrets`
- Modular project structure with separation of concerns
- FluentAI.NET SDK integration with proper dependency injection
- **Multi-provider AI support**: OpenAI, Google Gemini, and Anthropic Claude
- Comprehensive error handling and user guidance
- Support for multiple configuration sources with priority ordering
- Provider failover mechanism with automatic fallback

## Prerequisites

- .NET 8.0 or later
- Valid API key from any supported provider (OpenAI, Google, Anthropic) for actual functionality

## Setup Instructions

### 1. Clone and Build

```bash
# Clone the repository
git clone <repository-url>
cd FluentAI.ConsoleApp

# Restore packages and build
dotnet restore
dotnet build
```

### 2. Configure API Key

You can configure your OpenAI API key using any of the following methods (in order of priority):

#### Option 1: appsettings.json (Recommended for development)

Edit the `appsettings.json` file:

```json
{
  "Aisdk": {
    "OpenAI": {
      "ApiKey": "your-actual-openai-api-key-here",
      "Model": "gpt-3.5-turbo"
    }
  }
}
```

#### Option 2: Environment Variables

Set the environment variable:

```bash
# Windows
set OPENAI_API_KEY=your-actual-openai-api-key-here

# Linux/macOS
export OPENAI_API_KEY=your-actual-openai-api-key-here

# Or use the alternative format
export OpenAI__ApiKey=your-actual-openai-api-key-here
```

#### Option 3: User Secrets (Recommended for development)

Use .NET user secrets to securely store your API key:

```bash
# Initialize user secrets (first time only)
dotnet user-secrets init

# Set your API key
dotnet user-secrets set "OPENAI_API_KEY" "your-actual-openai-api-key-here"

# Or use the alternative format
dotnet user-secrets set "OpenAI:ApiKey" "your-actual-openai-api-key-here"
```

### 3. Get an OpenAI API Key

1. Visit [OpenAI API Keys](https://platform.openai.com/api-keys)
2. Create an account or sign in
3. Generate a new API key
4. Configure it using one of the methods above

## Running the Application

### Interactive Chat Mode (Default)
```bash
dotnet run
```

### Test Modes
```bash
# Test configuration loading
dotnet run test-config

# Test backward compatibility
dotnet run test-legacy

# Test new AI assistant features
dotnet run test-features

# Run interactive demo (without requiring API keys)
dotnet run demo
```

## Interactive Usage

Once started, the application provides an interactive chat interface:

```
FluentAI.NET Advanced AI Assistant
==================================
Provider: OpenAI
Model: gpt-4
Context Window: 4000 tokens
Safety Features: Enabled
Conversation Memory: Enabled

Available Commands:
==================
:new    - Start a new conversation (clears history)
:del    - Delete your last message
:help   - Show this help message
:quit   - Exit the application (:q, :exit also work)

Chat started! Type your message or use commands starting with ':'
=====================================

You: Hello! How can you help me today?
```

### Example Interaction
```
You: Can you help me learn Python programming?

AI: I'd be happy to help you learn Python! Python is an excellent 
language for beginners. What specific area would you like to start 
with - basic syntax, data structures, or perhaps a particular project?

📊 Tokens: Input=12, Output=35, Total=127

You: :del
✓ Removed your last message.

You: :new  
✓ Started new conversation. Previous history cleared.
```

## Project Structure

```
FluentAI.ConsoleApp/
├── Configuration/
│   ├── AiSdkConfiguration.cs               # AI SDK and chat configuration models
│   └── ApiKeyConfigurationService.cs       # API key configuration with fallback logic
├── Services/
│   ├── ApplicationService.cs               # Main application entry point
│   ├── ConversationManager.cs              # In-memory conversation and token management
│   ├── ChatSessionManager.cs               # Interactive chat commands and session control
│   ├── InputValidationService.cs           # Safety features and input validation
│   ├── InteractiveChatService.cs           # Orchestrates interactive chat experience
│   └── FluentAiServiceSetup.cs            # FluentAI service configuration
├── Tests/
│   ├── AiAssistantFeaturesTest.cs          # Tests for new AI assistant features
│   ├── InteractiveChatDemo.cs              # Interactive demo without requiring API keys
│   ├── BackwardCompatibilityTest.cs        # Legacy configuration compatibility tests
│   └── ConfigurationTest.cs                # Configuration loading tests
├── Extensions/
│   └── FluentAiBuilderExtensions.cs        # FluentAI service registration extensions
├── FluentAI.ConsoleApp.csproj              # Project file with NuGet package references
├── Program.cs                              # Entry point with dependency injection
├── appsettings.json                        # Configuration file for API keys and chat settings
└── README.md                               # This file
```

## Configuration

### Chat Configuration

The application supports extensive chat configuration in `appsettings.json`:

```json
{
  "AiSdk": {
    "DefaultProvider": "OpenAI",
    "Failover": {
      "PrimaryProvider": "OpenAI",
      "FallbackProvider": "Google"
    },
    "Chat": {
      "ContextWindowTokens": 4000,
      "SystemPrompt": "You are a helpful, professional AI assistant...",
      "EnableSafetyFeatures": true,
      "EnableConversationMemory": true
    }
  }
}
```

### API Key Priority

The application follows this priority order for API key configuration:

1. **appsettings.json**: `Aisdk:OpenAI:ApiKey`
2. **Environment Variable**: `OPENAI_API_KEY`
3. **Environment Variable**: `OpenAI__ApiKey`
4. **User Secrets**: For development security
5. **Default**: Falls back to placeholder for demo purposes

## Key Code Components

### Interactive Chat Service

The `InteractiveChatService` orchestrates the entire chat experience:

```csharp
public async Task StartChatAsync()
{
    DisplayWelcomeMessage();
    var chatModel = await InitializeChatModel();
    
    // Add system message to conversation
    _conversationManager.AddMessage(ChatRole.System, systemPrompt);
    
    while (true)
    {
        await ProcessUserInput(chatModel, aiSdkConfig);
    }
}
```

### Conversation Management

The `ConversationManager` handles memory and token management:

```csharp
public class ConversationManager : IConversationManager
{
    public void AddMessage(ChatRole role, string content);
    public List<ChatMessage> GetMessagesWithSummary(int maxTokens);
    public void ClearConversation();
    public void RemoveLastUserMessage();
    public int EstimateTokenCount();
    public bool IsNearTokenLimit(int maxTokens);
}
```

### Safety Features

The `InputValidationService` provides comprehensive safety:

```csharp
public ValidationResult ValidateInput(string input)
{
    // Check for risky content, excessive length, etc.
    if (ContainsRiskyContent(input))
        return new ValidationResult(false, "Content cannot be processed...");
        
    // Check for vague inputs needing clarification
    if (IsVagueInput(input))
        return new ValidationResult(true, null, true, GetClarificationPrompt(input));
}
```

### Service Configuration

The `Program.cs` registers all AI assistant services:

```csharp
private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Core services
    services.AddSingleton<IApiKeyConfigurationService, ApiKeyConfigurationService>();
    services.AddSingleton<IApplicationService, ApplicationService>();
    
    // AI assistant services
    services.AddSingleton<IConversationManager, ConversationManager>();
    services.AddSingleton<IChatSessionManager, ChatSessionManager>();
    services.AddSingleton<IInputValidationService, InputValidationService>();
    services.AddSingleton<IInteractiveChatService, InteractiveChatService>();
    
    // FluentAI services
    services.AddFluentAiServices(configService);
}
```

## Expected Behavior

### With Default Configuration (No API Key Set)
The application will:
- Display the advanced AI assistant welcome screen
- Show configuration details (provider, model, context window, features)
- Attempt to connect to AI providers
- Gracefully handle connection failures with helpful guidance
- Provide detailed instructions on configuring API keys

### With Valid API Key
The application will:
- Successfully connect to the AI provider
- Start an interactive chat session
- Support all advanced features (memory, commands, safety)
- Provide real-time token usage information
- Handle conversation summarization automatically
The application will:
- Initialize successfully
- Show the configured API key (partially masked for security)
- Send the prompt to OpenAI
- Display the AI response
- Show token usage information

## Dependencies

- **FluentAI.NET (1.0.5)**: Main AI SDK package
- **Microsoft.Extensions.Configuration.Json (9.0.8)**: JSON configuration support
- **Microsoft.Extensions.Configuration.UserSecrets (9.0.8)**: User secrets support
- **Microsoft.Extensions.Configuration.EnvironmentVariables (9.0.8)**: Environment variables support
- **Microsoft.Extensions.Logging.Console (9.0.8)**: Console logging support
- **.NET 8.0**: Target framework

## Architecture Notes

- **Separation of Concerns**: Distinct layers for configuration, conversation management, safety, and UI
- **Dependency Injection**: Uses Microsoft.Extensions.DependencyInjection for service management
- **Configuration Pattern**: Implements .NET configuration with multiple providers and fallback logic
- **Modular Design**: Each feature (memory, safety, commands) is in separate services
- **Extensible Architecture**: Easy to add new AI providers, commands, or safety features
- **Comprehensive Error Handling**: User-friendly guidance and graceful failure handling

## Security Considerations

- **Input Validation**: Comprehensive validation and sanitization of user inputs
- **Content Filtering**: Prevents processing of harmful or inappropriate content
- **Prompt Injection Protection**: Guards against malicious prompt manipulation attempts
- **API Key Protection**: Keys are never logged in full (only first 10 characters shown)
- **User Secrets**: Recommended for development to keep API keys out of source control
- **Environment Variables**: Suitable for production deployment scenarios
- **Safe Configuration**: Critical settings are validated and have secure defaults

## Advanced Features Implemented

✅ **Conversation Memory** - Full session memory with intelligent summarization  
✅ **Token Management** - Real-time tracking and automatic optimization  
✅ **Context Awareness** - Seamless integration of history and summaries  
✅ **Safety Features** - Input validation, content filtering, risk assessment  
✅ **Interactive Commands** - Vim-style commands for session management  
✅ **Response Quality** - Professional, clear responses with clarification prompts  
✅ **Multi-provider Support** - OpenAI, Google, Anthropic with failover  
✅ **Extensible Architecture** - Easy to add new features and providers

## Testing

The application includes comprehensive tests:

```bash
# Test all new AI assistant features
dotnet run test-features

# Run interactive demo (no API keys required)
dotnet run demo

# Test configuration system
dotnet run test-config

# Test backward compatibility
dotnet run test-legacy
```