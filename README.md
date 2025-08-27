# LightTrace

A lightweight tracing library for .NET applications that provides simple, efficient performance monitoring and diagnostics.

## Features

- ⚡ **Lightweight**: Minimal overhead with high-performance tracing
- 🔄 **Async/Await Support**: Thread-safe tracing with AsyncLocal context preservation
- 🌳 **Hierarchical Tracing**: Support for nested trace operations
- 📊 **Markdown Reports**: Automatically generated trace reports in markdown format
- ⚙️ **Configurable**: Environment variable-based configuration
- 🎯 **Simple API**: Easy-to-use disposable pattern with `using` statements
- 🌐 **.NET Standard 2.0**: Compatible with .NET Framework, .NET Core, and .NET 5+

## Installation

### NuGet Package

```bash
dotnet add package LightTrace
```

### Package Manager Console

```powershell
Install-Package LightTrace
```

## Quick Start

```csharp
using LightTrace;

// Simple tracing with using statement
using (new Tracer("DatabaseOperation"))
{
    // Your code here
    await database.SaveAsync(data);
}

// Nested tracing for detailed performance analysis
using (new Tracer("ProcessOrder"))
{
    using (new Tracer("ValidateOrder"))
    {
        ValidateOrder(order);
    }
    
    using (new Tracer("SaveToDatabase"))
    {
        await SaveOrderAsync(order);
    }
    
    using (new Tracer("SendConfirmation"))
    {
        await SendEmailAsync(order.Email);
    }
}
```

## Advanced Usage

### Parallel Operations

LightTrace is designed to work seamlessly with parallel operations and maintains separate trace contexts for each execution path:

```csharp
Parallel.ForEach(orders, order =>
{
    using (new Tracer("ProcessOrder"))
    {
        using (new Tracer("Validation"))
        {
            ValidateOrder(order);
        }
        
        using (new Tracer("Persistence"))
        {
            SaveOrder(order);
        }
    }
});
```

### Getting Trace Results

```csharp
// Get current trace snapshot
var traces = Tracer.GetTraceEntries();

// Convert to markdown report
string markdownReport = traces.AsMdReportString();
Console.WriteLine(markdownReport);
```

### Example Output

The trace report shows execution times and call counts in a hierarchical format:

| Path | Time | Count |
| --- | --- | --- |
| --- ProcessOrder | 150.2ms | 4 |
| ---  --- Validation | 45.1ms | 4 |
| ---  --- Persistence | 95.8ms | 4 |
| ---  --- SendConfirmation | 8.3ms | 4 |

## Automatic Reporting

LightTrace automatically generates trace reports in the background. Reports are saved as markdown files and updated periodically.

### Report Configuration

Configure reporting behavior using environment variables:

```bash
# Set report interval (in seconds, default: 15)
set LIGHT_TRACE_REPORT_INTERVAL=30

# Set report folder (default: %TEMP%)
set LIGHT_TRACE_REPORT_FOLDER=C:\Logs\Traces
```

### Report File Location

Reports are automatically saved to: `{ReportFolder}\{ProcessName}_Traces.md`

Access the current report file path:

```csharp
string reportPath = TraceReport.ReportFile;
Console.WriteLine($"Traces are being saved to: {reportPath}");
```

## Configuration

| Environment Variable | Description | Default Value |
|---------------------|-------------|---------------|
| `LIGHT_TRACE_REPORT_INTERVAL` | Report generation interval in seconds | `15` |
| `LIGHT_TRACE_REPORT_FOLDER` | Directory for trace report files | `%TEMP%` |

## API Reference

### Tracer Class

The main tracing class that implements `IDisposable` for automatic timing measurement.

```csharp
public sealed class Tracer : IDisposable
{
    public Tracer(string name)  // Creates and starts a new trace
    public void Dispose()       // Stops the trace and records timing
    public static TraceEntrySnapshots GetTraceEntries() // Gets current trace data
}
```

### TraceReport Class

Handles automatic background reporting of trace data.

```csharp
public class TraceReport
{
    public static string ReportFile { get; }  // Current report file path
    public static void Start()                // Starts background reporting (called automatically)
}
```

### Extension Methods

```csharp
// Convert trace data to markdown format
public static string AsMdReportString(this TraceEntrySnapshots traces)
public static IEnumerable<string> AsMdReport(this TraceEntrySnapshots traces)
```

## Performance Characteristics

- **Minimal Overhead**: Uses high-resolution `Stopwatch` for accurate timing
- **Thread-Safe**: Concurrent operations are handled safely with `ConcurrentDictionary` and `Interlocked` operations
- **Memory Efficient**: Traces are aggregated by name to minimize memory usage
- **Async-Aware**: Maintains separate trace contexts per logical execution flow

## Use Cases

- **Performance Monitoring**: Identify bottlenecks in your application
- **Debugging**: Understand execution flow and timing
- **Load Testing**: Monitor performance under various loads
- **Production Monitoring**: Lightweight performance tracking in production environments
- **API Performance**: Track web API endpoint performance
- **Database Operations**: Monitor query and transaction performance

## Examples

### Web API Controller

```csharp
[ApiController]
public class OrderController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        using (new Tracer("CreateOrder"))
        {
            using (new Tracer("ValidateOrder"))
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
            }
            
            using (new Tracer("SaveOrder"))
            {
                await _orderService.SaveAsync(order);
            }
            
            using (new Tracer("SendNotification"))
            {
                await _notificationService.SendOrderConfirmationAsync(order);
            }
            
            return Ok(order);
        }
    }
}
```

### Background Service

```csharp
public class OrderProcessingService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (new Tracer("ProcessPendingOrders"))
            {
                var orders = await GetPendingOrdersAsync();
                
                await Parallel.ForEachAsync(orders, stoppingToken, async (order, ct) =>
                {
                    using (new Tracer("ProcessSingleOrder"))
                    {
                        await ProcessOrderAsync(order, ct);
                    }
                });
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
```

## Requirements

- .NET Standard 2.0 or higher
- .NET Framework 4.6.1+ / .NET Core 2.0+ / .NET 5+

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

If you encounter any issues or have questions, please [open an issue](https://github.com/vantonenko/LightTrace/issues) on GitHub.