# FluentAI.NET Console Application

A C# .NET console application demonstrating the usage of the FluentAI.NET NuGet package (version 1.0.3) with flexible API key configuration and modular architecture.

## Features

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

```bash
dotnet run
```

## Project Structure

```
FluentAI.ConsoleApp/
├── Configuration/
│   └── ApiKeyConfigurationService.cs    # API key configuration with fallback logic
├── Services/
│   ├── ApplicationService.cs            # Main application logic
│   └── FluentAiServiceSetup.cs         # FluentAI service configuration
├── FluentAI.ConsoleApp.csproj          # Project file with NuGet package references
├── Program.cs                          # Minimal entry point with dependency injection
├── appsettings.json                    # Configuration file for API keys and settings
└── README.md                           # This file
```

## Configuration Priority

The application follows this priority order for API key configuration:

1. **appsettings.json**: `Aisdk:OpenAI:ApiKey`
2. **Environment Variable**: `OPENAI_API_KEY`
3. **Environment Variable**: `OpenAI__ApiKey`
4. **Default**: Falls back to placeholder for demo purposes

## Key Code Components

### Configuration Service

The `ApiKeyConfigurationService` handles API key retrieval with automatic fallback:

```csharp
public string GetApiKey()
{
    // Try appsettings.json first
    var apiKey = _configuration["Aisdk:OpenAI:ApiKey"];
    if (!string.IsNullOrWhiteSpace(apiKey)) return apiKey;

    // Fallback to environment variables
    apiKey = _configuration["OPENAI_API_KEY"];
    if (!string.IsNullOrWhiteSpace(apiKey)) return apiKey;

    // Final fallback
    return "your-openai-api-key-here";
}
```

### Service Configuration

The `Program.cs` file now handles only essential startup logic:

```csharp
private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddSingleton(configuration);
    services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
    services.AddSingleton<IApiKeyConfigurationService, ApiKeyConfigurationService>();
    services.AddSingleton<IApplicationService, ApplicationService>();
    
    var configService = new ApiKeyConfigurationService(configuration);
    services.AddFluentAiServices(configService);
}
```

### Application Logic

The main application logic is now in `ApplicationService`:

```csharp
public async Task RunAsync()
{
    // Get configuration
    var apiKey = _configService.GetApiKey();
    var model = _configService.GetModel();

    // Use FluentAI services
    var chatModelFactory = _serviceProvider.GetRequiredService<IChatModelFactory>();
    var chatModel = chatModelFactory.GetModel("OpenAI");
    
    // Send prompt and handle response...
}
```

## Expected Behavior

### With Default Configuration (No API Key Set)
The application will:
- Initialize successfully
- Show configuration details with placeholder API key
- Attempt to send the prompt
- Display a network/authentication error (expected)
- Provide detailed instructions on how to configure a real API key using any of the supported methods

### With Valid API Key (Any Configuration Method)
The application will:
- Initialize successfully
- Show the configured API key (partially masked for security)
- Send the prompt to OpenAI
- Display the AI response
- Show token usage information

## Dependencies

- **FluentAI.NET (1.0.3)**: Main AI SDK package
- **Microsoft.Extensions.Configuration.Json (9.0.8)**: JSON configuration support
- **Microsoft.Extensions.Configuration.UserSecrets (9.0.8)**: User secrets support
- **Microsoft.Extensions.Configuration.EnvironmentVariables (9.0.8)**: Environment variables support
- **Microsoft.Extensions.Logging.Console (9.0.8)**: Console logging support
- **.NET 8.0**: Target framework

## Architecture Notes

- **Separation of Concerns**: The application is structured with distinct layers for configuration, services, and application logic
- **Dependency Injection**: Uses Microsoft.Extensions.DependencyInjection for proper service management
- **Configuration Pattern**: Implements the .NET configuration pattern with multiple providers and fallback logic
- **Extensible Design**: Easy to add new AI providers or configuration sources
- **Error Handling**: Comprehensive error handling with user-friendly guidance

## Security Considerations

- **API Key Protection**: Keys are never logged in full (only first 10 characters shown)
- **User Secrets**: Recommended for development to keep API keys out of source control
- **Environment Variables**: Suitable for production deployment scenarios
- **Configuration Files**: Should be excluded from source control when containing real API keys

## Next Steps

To extend this application, you could:
- Add streaming response support
- Implement conversation history management
- Implement request/response validation and sanitization
- Add configuration for model parameters (temperature, max tokens, etc.)
- Create a more sophisticated CLI interface with commands and options