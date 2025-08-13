using FluentAI.Abstractions;
using FluentAI.Abstractions.Models;
using FluentAI.Abstractions.Performance;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FluentAI.Examples.ConsoleApp;

/// <summary>
/// Demonstrates performance features including caching, monitoring, and metrics.
/// </summary>
public class PerformanceDemoService
{
    private readonly IChatModel _chatModel;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PerformanceDemoService> _logger;

    public PerformanceDemoService(
        IChatModel chatModel,
        IServiceProvider serviceProvider,
        ILogger<PerformanceDemoService> logger)
    {
        _chatModel = chatModel;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task RunPerformanceDemo()
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                  Performance Features Demo                  ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        await RunCachingDemo();
        Console.WriteLine();
        await RunPerformanceMonitoringDemo();
        Console.WriteLine();
        await RunMemoryManagementDemo();
        Console.WriteLine();
        await RunBenchmarkDemo();
    }

    private async Task RunCachingDemo()
    {
        Console.WriteLine("💾 Response Caching Demo:");
        Console.WriteLine("   ─────────────────────");
        Console.WriteLine();

        var cache = _serviceProvider.GetService<IResponseCache>();
        if (cache == null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠️ Response cache not configured. Showing timing comparison...");
            Console.ResetColor();
            await RunCachingBenchmark();
            return;
        }

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a helpful assistant."),
            new(ChatRole.User, "What is the capital of France?")
        };

        Console.WriteLine("🔄 First request (no cache):");
        var startTime = DateTime.UtcNow;
        
        // Check if cached response exists
        var cachedResponse = await cache.GetAsync(messages);
        if (cachedResponse != null)
        {
            Console.WriteLine("✅ Found cached response!");
            Console.WriteLine($"📝 Cached content: {TruncateText(cachedResponse.Content, 100)}");
        }
        else
        {
            Console.WriteLine("📤 Making API request...");
            var response = await _chatModel.GetResponseAsync(messages);
            var duration = DateTime.UtcNow - startTime;
            
            Console.WriteLine($"⏱️ Response time: {duration.TotalMilliseconds:F0}ms");
            Console.WriteLine($"📝 Response: {TruncateText(response.Content, 100)}");
            
            // Cache the response
            await cache.SetAsync(messages, null, response, TimeSpan.FromMinutes(5));
            Console.WriteLine("💾 Response cached for 5 minutes");
        }

        Console.WriteLine();
        Console.WriteLine("🔄 Second request (with cache):");
        startTime = DateTime.UtcNow;
        cachedResponse = await cache.GetAsync(messages);
        var cacheTime = DateTime.UtcNow - startTime;

        if (cachedResponse != null)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"⚡ Cache hit! Retrieved in {cacheTime.TotalMilliseconds:F1}ms");
            Console.ResetColor();
            Console.WriteLine($"📝 Cached content: {TruncateText(cachedResponse.Content, 100)}");
        }
        else
        {
            Console.WriteLine("❌ Cache miss (unexpected)");
        }
    }

    private async Task RunCachingBenchmark()
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, "Tell me about C# programming language.")
        };

        Console.WriteLine("📊 Performance comparison (simulated caching benefits):");
        Console.WriteLine();

        // First request timing
        Console.WriteLine("🔄 First request (API call):");
        var startTime = DateTime.UtcNow;
        var response = await _chatModel.GetResponseAsync(messages);
        var apiTime = DateTime.UtcNow - startTime;

        Console.WriteLine($"⏱️ API response time: {apiTime.TotalMilliseconds:F0}ms");
        Console.WriteLine($"📝 Response: {TruncateText(response.Content, 100)}");
        Console.WriteLine();

        // Simulate cache retrieval time
        Console.WriteLine("🔄 Subsequent requests (with caching):");
        var cacheTime = TimeSpan.FromMilliseconds(2); // Typical cache retrieval time
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"⚡ Cache retrieval time: {cacheTime.TotalMilliseconds:F1}ms");
        Console.WriteLine($"🚀 Performance improvement: {(apiTime.TotalMilliseconds / cacheTime.TotalMilliseconds):F0}x faster");
        Console.ResetColor();
    }

    private async Task RunPerformanceMonitoringDemo()
    {
        Console.WriteLine("📈 Performance Monitoring Demo:");
        Console.WriteLine("   ──────────────────────────────");
        Console.WriteLine();

        var monitor = _serviceProvider.GetService<IPerformanceMonitor>();
        if (monitor == null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠️ Performance monitor not configured. Showing manual timing...");
            Console.ResetColor();
            await RunManualPerformanceTracking();
            return;
        }

        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, "Explain machine learning in 50 words.")
        };

        Console.WriteLine("📊 Starting performance monitoring...");

        // Start operation timing
        using (monitor.StartOperation("ChatCompletion"))
        {
            var response = await _chatModel.GetResponseAsync(messages);
            
            // Record custom metrics
            monitor.RecordMetric("ResponseLength", response.Content.Length);
            monitor.RecordMetric("InputTokens", response.Usage.InputTokens);
            monitor.RecordMetric("OutputTokens", response.Usage.OutputTokens);
            monitor.IncrementCounter("CompletedRequests");

            Console.WriteLine($"📝 Response: {TruncateText(response.Content, 100)}");
        }

        // Get operation statistics
        var stats = monitor.GetOperationStats("ChatCompletion");
        if (stats != null)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("📈 Performance Statistics:");
            Console.WriteLine($"   • Operation Count: {stats.ExecutionCount}");
            Console.WriteLine($"   • Average Duration: {stats.AverageExecutionTimeMs:F1}ms");
            Console.WriteLine($"   • Min Duration: {stats.MinExecutionTimeMs:F1}ms");
            Console.WriteLine($"   • Max Duration: {stats.MaxExecutionTimeMs:F1}ms");
            Console.WriteLine($"   • Success Rate: {stats.SuccessRate:F1}%");
            Console.ResetColor();
        }
    }

    private async Task RunManualPerformanceTracking()
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, "What are the benefits of performance monitoring?")
        };

        Console.WriteLine("⏱️ Manual performance tracking:");
        var startTime = DateTime.UtcNow;
        var memoryBefore = GC.GetTotalMemory(false);

        var response = await _chatModel.GetResponseAsync(messages);

        var endTime = DateTime.UtcNow;
        var memoryAfter = GC.GetTotalMemory(false);
        var duration = endTime - startTime;
        var memoryUsed = memoryAfter - memoryBefore;

        Console.WriteLine($"📝 Response: {TruncateText(response.Content, 100)}");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("📊 Performance Metrics:");
        Console.WriteLine($"   • Duration: {duration.TotalMilliseconds:F1}ms");
        Console.WriteLine($"   • Memory Delta: {memoryUsed:N0} bytes");
        Console.WriteLine($"   • Response Length: {response.Content.Length} characters");
        Console.WriteLine($"   • Token Usage: {response.Usage.InputTokens} → {response.Usage.OutputTokens}");
        Console.ResetColor();
    }

    private async Task RunMemoryManagementDemo()
    {
        Console.WriteLine("🧠 Memory Management Demo:");
        Console.WriteLine("   ──────────────────────");
        Console.WriteLine();

        var initialMemory = GC.GetTotalMemory(false);
        Console.WriteLine($"📊 Initial memory usage: {initialMemory:N0} bytes");

        // Simulate memory-intensive operations
        var responses = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            var messages = new List<ChatMessage>
            {
                new(ChatRole.User, $"Generate a list of {10 + i * 5} programming concepts.")
            };

            var response = await _chatModel.GetResponseAsync(messages);
            responses.Add(response.Content);
            
            var currentMemory = GC.GetTotalMemory(false);
            Console.WriteLine($"🔄 Request {i + 1}: {currentMemory:N0} bytes (+{currentMemory - initialMemory:N0})");
        }

        Console.WriteLine();
        Console.WriteLine("🗑️ Triggering garbage collection...");
        
        // Clear references and force GC
        responses.Clear();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var finalMemory = GC.GetTotalMemory(false);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✅ Memory after cleanup: {finalMemory:N0} bytes");
        Console.WriteLine($"♻️ Memory reclaimed: {initialMemory - finalMemory + (finalMemory > initialMemory ? finalMemory - initialMemory : 0):N0} bytes");
        Console.ResetColor();
    }

    private async Task RunBenchmarkDemo()
    {
        Console.WriteLine("🏁 Benchmark Demo:");
        Console.WriteLine("   ─────────────────");
        Console.WriteLine();

        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, "Write a haiku about programming.")
        };

        Console.WriteLine("📊 Running benchmark with 3 requests...");
        var times = new List<TimeSpan>();

        for (int i = 1; i <= 3; i++)
        {
            Console.Write($"🔄 Request {i}: ");
            var startTime = DateTime.UtcNow;
            
            var response = await _chatModel.GetResponseAsync(messages);
            var duration = DateTime.UtcNow - startTime;
            times.Add(duration);
            
            Console.WriteLine($"{duration.TotalMilliseconds:F0}ms");
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("📈 Benchmark Results:");
        Console.WriteLine($"   • Average: {times.Average(t => t.TotalMilliseconds):F1}ms");
        Console.WriteLine($"   • Fastest: {times.Min(t => t.TotalMilliseconds):F1}ms");
        Console.WriteLine($"   • Slowest: {times.Max(t => t.TotalMilliseconds):F1}ms");
        Console.WriteLine($"   • Std Dev: {CalculateStandardDeviation(times):F1}ms");
        Console.ResetColor();
    }

    private double CalculateStandardDeviation(IEnumerable<TimeSpan> times)
    {
        var values = times.Select(t => t.TotalMilliseconds).ToArray();
        var average = values.Average();
        var sumOfSquares = values.Sum(v => Math.Pow(v - average, 2));
        return Math.Sqrt(sumOfSquares / values.Length);
    }

    private string TruncateText(string text, int maxLength)
    {
        if (text.Length <= maxLength)
            return text;
        
        return text.Substring(0, maxLength) + "...";
    }
}