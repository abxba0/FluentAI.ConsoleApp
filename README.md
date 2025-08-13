# FluentAI.NET Console Application

A minimal C# .NET console application demonstrating the usage of the FluentAI.NET NuGet package (version 1.0.3).

## Features

- Minimal project structure with a single `Program.cs` file
- FluentAI.NET SDK initialization with placeholder API key
- Demonstrates sending prompts and receiving responses from AI models
- Proper error handling and user guidance
- Uses dependency injection pattern with logging support

## Prerequisites

- .NET 8.0 or later
- Valid OpenAI API key (for actual functionality)

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

### 2. Add FluentAI.NET Package (Already Included)

The project already includes the FluentAI.NET package, but if you were setting this up from scratch:

```bash
dotnet add package FluentAI.NET --version 1.0.3
dotnet add package Microsoft.Extensions.Logging.Console
```

### 3. Configure API Key

To use the application with a real API key:

1. Visit [OpenAI API Keys](https://platform.openai.com/api-keys)
2. Create an account or sign in
3. Generate a new API key
4. Replace the placeholder in `Program.cs`:

```csharp
var apiKey = "your-actual-openai-api-key-here";
```

## Running the Application

```bash
dotnet run
```

## Project Structure

```
FluentAI.ConsoleApp/
├── FluentAI.ConsoleApp.csproj  # Project file with NuGet package references
├── Program.cs                  # Main application code
└── README.md                   # This file
```

## Key Code Components

### SDK Initialization

```csharp
// Create service collection and configure FluentAI
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
```

### Sending Prompts

```csharp
// Get the chat model factory
var chatModelFactory = serviceProvider.GetRequiredService<IChatModelFactory>();
var chatModel = chatModelFactory.GetModel("OpenAI");

// Create chat messages
var messages = new List<ChatMessage>
{
    new ChatMessage(ChatRole.User, prompt)
};

// Send prompt and get response
var requestOptions = new OpenAiRequestOptions();
var response = await chatModel.GetResponseAsync(messages, requestOptions, CancellationToken.None);
```

## Expected Behavior

### With Placeholder API Key
The application will:
- Initialize successfully
- Show configuration details
- Attempt to send the prompt
- Display an authentication error (expected)
- Provide instructions on how to get a real API key

### With Valid API Key
The application will:
- Initialize successfully
- Send the prompt to OpenAI
- Display the AI response
- Show token usage information

## Dependencies

- **FluentAI.NET (1.0.3)**: Main AI SDK package
- **Microsoft.Extensions.Logging.Console**: Console logging support
- **.NET 8.0**: Target framework

## Notes

- This is a minimal example focused on demonstrating basic FluentAI.NET usage
- The project uses dependency injection patterns recommended by FluentAI.NET
- Error handling includes helpful user guidance
- Logging is configured to reduce noise (Warning level and above)

## Next Steps

To extend this application, you could:
- Add configuration file support for API keys
- Implement multiple AI providers (Anthropic, Google, etc.)
- Add conversation history management
- Implement streaming responses
- Add input validation and sanitization