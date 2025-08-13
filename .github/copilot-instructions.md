# FluentAI.NET Console Application Development Guide

**ALWAYS follow these instructions first. Only use additional search or context gathering if the information here is incomplete or found to be in error.**

## Project Overview

FluentAI.NET Console Application is a .NET 8.0 console application that demonstrates the comprehensive capabilities of the FluentAI.NET SDK through an interactive menu system. It supports multiple AI providers (OpenAI, Anthropic, Google AI, HuggingFace) and showcases security, performance, configuration, and error handling features.

## Working Effectively

### Prerequisites and Setup
- Ensure .NET 8.0 SDK is installed
- Navigate to the project root directory first:
  ```bash
  cd /home/runner/work/FluentAI.ConsoleApp/FluentAI.ConsoleApp
  ```

### Build and Restore Commands
Run these commands in sequence for initial setup:

1. **Restore NuGet packages** - NEVER CANCEL: Takes ~20 seconds. Set timeout to 60+ minutes:
   ```bash
   dotnet restore
   ```

2. **Build the application** - NEVER CANCEL: Takes ~7 seconds. Set timeout to 30+ minutes:
   ```bash
   dotnet build
   ```
   
   Expected output: Build will succeed with 12 warnings (this is normal). Warnings are related to:
   - CS1998: Async methods without await operators in EdgeCaseTestService
   - CS8620: Nullability warnings in configuration code
   - CS8619: Nullability warnings in ErrorHandlingDemoService

### Running the Application

**CRITICAL**: The application requires proper AI provider configuration to run successfully.

#### Without API Keys (Expected Failure Mode)
```bash
dotnet run --no-build
```
Expected error: `❌ Service Validation Error: Cannot create default input sanitizer without compatible logger`

This is NORMAL behavior - the app cannot start without proper FluentAI SDK configuration. This error occurs during dependency injection setup, not API key validation.

#### With API Keys (Functional Testing Mode)
**Note**: Due to FluentAI SDK dependency injection requirements, the application currently shows the same initialization error even with valid API keys. This is a known limitation. The instructions below document the intended configuration workflow.

Configure API keys using **user secrets** (recommended) or environment variables:

**User Secrets Setup:**
```bash
# Verify user secrets is configured (should show no errors):
dotnet user-secrets list

# Set API keys for providers you plan to use:
dotnet user-secrets set "OPENAI_API_KEY" "your-actual-openai-api-key"
dotnet user-secrets set "ANTHROPIC_API_KEY" "your-actual-anthropic-api-key"  
dotnet user-secrets set "GOOGLE_API_KEY" "your-actual-google-api-key"
dotnet user-secrets set "HUGGINGFACE_API_KEY" "your-actual-huggingface-api-key"

# Run the application:
dotnet run --no-build
```

**Environment Variables (Alternative):**
```bash
# Linux/macOS:
export OPENAI_API_KEY="your-openai-api-key"
export ANTHROPIC_API_KEY="your-anthropic-api-key"
dotnet run --no-build

# Windows PowerShell:
$env:OPENAI_API_KEY="your-openai-api-key"
$env:ANTHROPIC_API_KEY="your-anthropic-api-key"
dotnet run --no-build
```

## Validation and Testing

### Manual Validation Scenarios
After making code changes, ALWAYS test these scenarios:

1. **Build Validation**: Ensure the project builds without errors
2. **Configuration Validation**: Run without API keys to ensure proper error messages  
3. **User Secrets Testing**: Test user secrets set/list/remove functionality
4. **Debug Mode**: Test with `--debug` flag for detailed error information
5. **Code Review**: Review Service classes for functional correctness

**Note**: Due to current FluentAI SDK limitations, full functional testing through the interactive menu is not possible. Focus on build validation and configuration testing.

### Validation Commands
```bash
# Test build process:
dotnet build

# Test configuration validation (should show clear error messages):
dotnet run --no-build

# Test with debug flag for detailed stack traces:
echo "0" | dotnet run --no-build -- --debug

# Test user secrets functionality:
dotnet user-secrets set "OPENAI_API_KEY" "test-key"
dotnet user-secrets list
dotnet user-secrets remove "OPENAI_API_KEY"
```

### Edge Case Testing
The application includes built-in edge case testing via `EdgeCaseTestService`:
- Designed to be accessed through main menu option 10: "Edge Case & Error Scenario Tests"
- Tests configuration errors, provider validation, and input sanitization  
- **Currently inaccessible**: Due to FluentAI SDK initialization issues, the interactive menu cannot be reached
- Code can be reviewed directly in `Services/EdgeCaseTestService.cs` for testing scenarios

## Configuration Management

### User Secrets Location
User secrets are stored at:
- **Windows**: `%APPDATA%\Microsoft\UserSecrets\fluentai-examples-consoleapp-secrets\secrets.json`
- **macOS/Linux**: `~/.microsoft/usersecrets/fluentai-examples-consoleapp-secrets/secrets.json`

### Managing User Secrets
```bash
# View all configured secrets:
dotnet user-secrets list

# Remove a specific secret:
dotnet user-secrets remove "OPENAI_API_KEY"

# Clear all secrets:
dotnet user-secrets clear
```

### Configuration Files
- **appsettings.json**: Contains provider settings, timeouts, rate limits, and failover configuration
- **FluentAI.Examples.ConsoleApp.csproj**: Project file with UserSecretsId configured
- No linting configuration files are present in this project

## Architecture and Key Files

### Main Components
- **Program.cs**: Entry point with dependency injection setup and service registration
- **Services/**: Directory containing all demo service implementations
  - `DemoService.cs`: Main menu coordinator
  - `ProviderDemoService.cs`: Multi-provider demonstrations  
  - `SecurityDemoService.cs`: Security feature showcase
  - `PerformanceDemoService.cs`: Performance and caching demos
  - `ConfigurationDemoService.cs`: Configuration management examples
  - `ErrorHandlingDemoService.cs`: Resilience and error handling
  - `EdgeCaseTestService.cs`: Built-in validation and error testing

### Dependencies
- **.NET 8.0**: Target framework
- **FluentAI.NET v1.0.2**: Main AI SDK package
- **Microsoft.Extensions.Hosting v8.0.0**: Dependency injection and hosting
- **Microsoft.Extensions.Configuration.Json v8.0.0**: Configuration management

## Common Issues and Troubleshooting

### Build Issues
- **Missing .NET 8.0**: Install .NET 8.0 SDK
- **Package restore failures**: Run `dotnet restore` with extended timeout

### Runtime Issues
- **"Cannot create default input sanitizer"**: FluentAI SDK dependency injection configuration issue. This is the current expected behavior regardless of API key configuration.
- **"Configuration Error"**: Check `appsettings.json` AiSdk section and environment variables
- **"Service Validation Error"**: This error occurs during FluentAI SDK initialization and is currently expected

### API Key Issues
- **User secrets not found**: Ensure you're in the correct directory and UserSecretsId exists in .csproj
- **Invalid API keys**: Use actual API keys from AI provider dashboards
- **Environment variable conflicts**: User secrets take precedence over environment variables

## Development Workflow

### Making Changes
1. **Always build and test before making changes**:
   ```bash
   dotnet build
   ```

2. **Test configuration behavior**: 
   ```bash
   dotnet run --no-build  # Should show FluentAI SDK initialization error
   ```

3. **Test user secrets functionality**: Verify user secrets commands work correctly

4. **Review code changes**: Since interactive testing is limited, focus on code review and build validation

5. **Test with debug mode**: Use `--debug` flag to see detailed error information

### No Traditional Unit Tests
This project does not use traditional unit testing frameworks. Instead:
- Review the built-in EdgeCaseTestService code in `Services/EdgeCaseTestService.cs`
- Manual testing through build and configuration validation
- Focus on code review and static analysis

**Note**: Interactive menu testing is currently not possible due to FluentAI SDK initialization issues.

### No CI/CD Pipeline
No GitHub Actions workflows exist. All validation is manual:
- Build the project locally
- Test runtime scenarios manually
- Verify configuration behavior

## Time Expectations

- **Package Restore**: ~20 seconds (NEVER CANCEL - allow up to 60 minutes)
- **Build**: ~7 seconds (NEVER CANCEL - allow up to 30 minutes)  
- **Startup**: 1-2 seconds for initialization
- **Configuration Validation**: Immediate feedback on startup

## Security Notes

- **NEVER commit API keys to source control**
- **Use user secrets for development**
- **Use secure configuration for production** (Azure Key Vault, etc.)
- **Regularly rotate API keys**
- **Monitor for exposed secrets in repositories**

User secrets are automatically excluded from version control and stored securely on the local machine with appropriate permissions.