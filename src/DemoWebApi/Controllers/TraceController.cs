using LightTrace;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TraceController : ControllerBase
{
    [HttpGet("report")]
    public ActionResult GetTraceReport()
    {
        using var tracer = new Tracer("TraceController.GetTraceReport");
        
        try
        {
            var traceEntries = Tracer.GetTraceEntries();
            return Ok(traceEntries);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("report/file")]
    public ActionResult GetTraceReportFile()
    {
        using var tracer = new Tracer("TraceController.GetTraceReportFile");
        
        try
        {
            if (!System.IO.File.Exists(TraceReport.ReportFile))
            {
                return NotFound(new { error = "Trace report file not found", path = TraceReport.ReportFile });
            }

            var content = System.IO.File.ReadAllText(TraceReport.ReportFile);
            return Ok(new { filePath = TraceReport.ReportFile, content });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("simulate-heavy-operation")]
    public async Task<ActionResult> SimulateHeavyOperation([FromQuery] int durationMs = 1000)
    {
        using var tracer = new Tracer($"TraceController.SimulateHeavyOperation(duration: {durationMs}ms)");
        
        try
        {
            // Simulate multiple nested operations
            await SimulateDataProcessingAsync(durationMs / 3);
            await SimulateBusinessLogicAsync(durationMs / 3);
            await SimulateExternalApiCallAsync(durationMs / 3);
            
            return Ok(new { message = $"Heavy operation completed in {durationMs}ms", timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    private async Task SimulateDataProcessingAsync(int duration)
    {
        using var tracer = new Tracer($"TraceController.SimulateDataProcessingAsync(duration: {duration}ms)");
        
        // Simulate data processing with sub-operations
        await SimulateSubOperation("ValidateData", duration / 4);
        await SimulateSubOperation("TransformData", duration / 2);
        await SimulateSubOperation("SaveData", duration / 4);
    }

    private async Task SimulateBusinessLogicAsync(int duration)
    {
        using var tracer = new Tracer($"TraceController.SimulateBusinessLogicAsync(duration: {duration}ms)");
        
        // Simulate business logic with sub-operations
        await SimulateSubOperation("ApplyBusinessRules", duration / 3);
        await SimulateSubOperation("CalculateMetrics", duration / 3);
        await SimulateSubOperation("GenerateReport", duration / 3);
    }

    private async Task SimulateExternalApiCallAsync(int duration)
    {
        using var tracer = new Tracer($"TraceController.SimulateExternalApiCallAsync(duration: {duration}ms)");
        
        // Simulate external API calls
        await SimulateSubOperation("AuthenticateWithApi", duration / 4);
        await SimulateSubOperation("SendApiRequest", duration / 2);
        await SimulateSubOperation("ProcessApiResponse", duration / 4);
    }

    private async Task SimulateSubOperation(string operationName, int duration)
    {
        using var tracer = new Tracer($"TraceController.{operationName}(duration: {duration}ms)");
        await Task.Delay(Math.Max(1, duration));
    }
}