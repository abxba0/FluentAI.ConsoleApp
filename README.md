# FluentAI.NET Console Example

This comprehensive console application demonstrates the full capabilities of the FluentAI.NET SDK through an interactive menu system.

## Features Demonstrated

### 🚀 Quick Start
```bash
cd Examples/ConsoleApp
dotnet run
```

### 📋 Demo Menu Options

1. **💬 Basic Chat Completion Demo**
   - Simple request/response interaction
   - Token usage and model information display
   - Error handling demonstration

2. **🌊 Streaming Response Demo**
   - Real-time token-by-token streaming
   - Visual streaming demonstration
   - Performance comparison with regular responses

3. **🔄 Multi-Provider Comparison**
   - Tests multiple prompts with the configured provider
   - Shows provider-specific features and capabilities
   - Demonstrates unified interface across providers

4. **🔒 Security Features Demo**
   - Input sanitization capabilities
   - Risk assessment and threat detection
   - Security best practices
   - Content filtering demonstrations

5. **⚡ Performance & Caching Demo**
   - Response caching mechanisms
   - Performance monitoring and metrics
   - Memory management demonstrations
   - Benchmark comparisons

6. **⚙️ Configuration Management Demo**
   - Current configuration display
   - Environment variable usage
   - Provider-specific configuration options
   - Configuration validation

7. **🚨 Error Handling & Resilience Demo**
   - Comprehensive error scenarios
   - Retry mechanisms with exponential backoff
   - Rate limiting demonstrations
   - Failover capabilities
   - Validation error handling

8. **🔧 Advanced Features Demo**
   - Overview of all SDK capabilities
   - Feature matrix and descriptions
   - Integration patterns

9. **💻 Interactive Chat (Original Demo)**
   - Free-form chat interface
   - Streaming mode toggle
   - Conversation history maintenance

## Configuration

### API Keys Setup

The FluentAI.NET Console Example supports multiple methods for configuring API keys:

#### Option 1: User Secrets (Recommended for Development)

User secrets provide a secure way to store API keys during development without exposing them in source code or environment variables.

##### Setup User Secrets

1. **Navigate to the ConsoleApp directory:**
   ```bash
   cd Examples/ConsoleApp
   ```

2. **Verify user secrets is configured** (should show no errors):
   ```bash
   dotnet user-secrets list
   ```
   
   Expected output when no secrets are configured:
   ```
   No secrets configured for this application.
   ```

3. **Add your API keys using user secrets:**
   ```bash
   # OpenAI
   dotnet user-secrets set "OPENAI_API_KEY" "your-actual-openai-api-key"
   
   # Anthropic
   dotnet user-secrets set "ANTHROPIC_API_KEY" "your-actual-anthropic-api-key"
   
   # Google AI
   dotnet user-secrets set "GOOGLE_API_KEY" "your-actual-google-api-key"
   
   # HuggingFace
   dotnet user-secrets set "HUGGINGFACE_API_KEY" "your-actual-huggingface-api-key"
   ```

4. **List all configured secrets** to verify:
   ```bash
   dotnet user-secrets list
   ```
   
   Example output:
   ```
   OPENAI_API_KEY = sk-proj-abcd1234...
   ANTHROPIC_API_KEY = sk-ant-api03-xyz...
   GOOGLE_API_KEY = AIzaSyABC123...
   HUGGINGFACE_API_KEY = hf_DEF456...
   ```

##### Managing User Secrets

- **View all secrets:** `dotnet user-secrets list`
- **Remove a specific secret:** `dotnet user-secrets remove "OPENAI_API_KEY"`
- **Clear all secrets:** `dotnet user-secrets clear`

##### Where Secrets Are Stored

User secrets are stored securely on your local machine:

- **Windows:** `%APPDATA%\Microsoft\UserSecrets\fluentai-examples-consoleapp-secrets\secrets.json`
- **macOS:** `~/.microsoft/usersecrets/fluentai-examples-consoleapp-secrets/secrets.json`
- **Linux:** `~/.microsoft/usersecrets/fluentai-examples-consoleapp-secrets/secrets.json`

**Security Benefits:**
- ✅ Secrets are stored outside your project directory
- ✅ Files are protected with appropriate permissions
- ✅ No risk of accidentally committing secrets to source control
- ✅ Each project has its own isolated secret store

#### Option 2: Environment Variables

Set your API keys as environment variables:

```bash
# OpenAI
export OPENAI_API_KEY="your-openai-api-key"

# Anthropic
export ANTHROPIC_API_KEY="your-anthropic-api-key"

# Google AI
export GOOGLE_API_KEY="your-google-api-key"

# HuggingFace
export HUGGINGFACE_API_KEY="your-huggingface-api-key"
```

**Windows PowerShell:**
```powershell
$env:OPENAI_API_KEY="your-openai-api-key"
$env:ANTHROPIC_API_KEY="your-anthropic-api-key"
$env:GOOGLE_API_KEY="your-google-api-key"
$env:HUGGINGFACE_API_KEY="your-huggingface-api-key"
```

**Windows Command Prompt:**
```cmd
set OPENAI_API_KEY=your-openai-api-key
set ANTHROPIC_API_KEY=your-anthropic-api-key
set GOOGLE_API_KEY=your-google-api-key
set HUGGINGFACE_API_KEY=your-huggingface-api-key
```

### Troubleshooting API Key Configuration

#### "No secrets configured for this application"

**Problem:** Running `dotnet user-secrets list` shows this message.

**Solutions:**
1. **Ensure you're in the correct directory:**
   ```bash
   cd Examples/ConsoleApp
   dotnet user-secrets list
   ```

2. **Verify UserSecretsId exists in project file:**
   Check that `FluentAI.Examples.ConsoleApp.csproj` contains:
   ```xml
   <UserSecretsId>fluentai-examples-consoleapp-secrets</UserSecretsId>
   ```

3. **Add secrets if none exist:**
   ```bash
   dotnet user-secrets set "OPENAI_API_KEY" "your_api_key_here"
   ```

#### "Could not find the global property 'UserSecretsId'"

**Problem:** The project doesn't have user secrets configured.

**Solution:** This should not occur with this project, but if it does:
1. Add the `<UserSecretsId>` property to the project file
2. Or use the `--id` parameter: `dotnet user-secrets set "KEY" "value" --id "fluentai-examples-consoleapp-secrets"`

#### Application doesn't recognize API keys

**Problem:** The application can't find your API keys even after setting them.

**Troubleshooting Steps:**
1. **Verify secrets are set:**
   ```bash
   dotnet user-secrets list
   ```

2. **Check configuration priority:** The application reads configuration in this order:
   - User secrets (development only)
   - Environment variables
   - appsettings.json

3. **Verify you're running in Development environment:**
   User secrets only work in the Development environment by default.

4. **Test with environment variable as fallback:**
   ```bash
   export OPENAI_API_KEY="your-key"
   dotnet run
   ```

#### Cross-Platform Considerations

- **File Permissions:** User secrets are automatically created with restricted permissions
- **Path Differences:** Storage paths vary by OS but functionality is identical
- **Environment Variables:** Syntax differs between shell types (bash vs PowerShell vs cmd)
- **Case Sensitivity:** Environment variable names are case-sensitive on Linux/macOS

### Security Best Practices

1. **Use User Secrets for Development:**
   - Never commit API keys to source control
   - User secrets are automatically excluded from version control

2. **Use Secure Configuration for Production:**
   - Azure Key Vault for cloud deployments
   - Environment variables on secure servers
   - Container orchestration secrets (Kubernetes, Docker Swarm)

3. **Regularly Rotate API Keys:**
   - Update secrets when team members leave
   - Use short-lived tokens when possible

4. **Monitor for Exposed Secrets:**
   - Regularly scan repositories for accidentally committed secrets
   - Use automated tools to detect exposed credentials

### Configuration Options
The application reads from `appsettings.json` which includes:

- **Provider Configuration**: Model selection, timeouts, token limits
- **Rate Limiting**: Request limits and time windows
- **Failover Settings**: Primary and fallback provider configuration
- **Logging**: Debug levels for different components

## Architecture

### Service Registration
The application demonstrates proper dependency injection setup:

```csharp
services.AddAiSdk(context.Configuration);
services.AddOpenAiChatModel(context.Configuration);
services.AddAnthropicChatModel(context.Configuration);
services.AddGoogleGeminiChatModel(context.Configuration);
services.AddHuggingFaceChatModel(context.Configuration);
```

### Demo Services
- **DemoService**: Main menu coordinator
- **ProviderDemoService**: Multi-provider demonstrations
- **SecurityDemoService**: Security feature showcase
- **PerformanceDemoService**: Performance and caching demos
- **ConfigurationDemoService**: Configuration management
- **ErrorHandlingDemoService**: Resilience and error handling

## Educational Value

This console application serves as:

1. **Learning Tool**: Understand all SDK features through hands-on demos
2. **Integration Example**: See how to properly set up and use FluentAI.NET
3. **Best Practices Guide**: Learn recommended patterns and configurations
4. **Troubleshooting Aid**: Test configurations and identify issues
5. **Feature Exploration**: Discover capabilities before integrating into projects

## Prerequisites

- .NET 8.0 or later
- At least one AI provider API key
- Internet connection for API calls

## Next Steps

After exploring the console demo:

1. Review the integration guides for your project type
2. Check the main SDK documentation for detailed API reference
3. Explore the test suite for additional implementation examples
4. Consider the security and performance features for production use

---

**Note**: Some demos require actual API keys to function fully. Without API keys, the application will still run and show the feature overviews and configurations, but won't make actual API calls.