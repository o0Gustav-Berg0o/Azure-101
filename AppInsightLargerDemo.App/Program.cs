using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Scalar.AspNetCore;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Application Insights (modern setup)
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddOpenApi();
// OpenAPI (för Scalar)
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<TelemetryClient>();

var app = builder.Build();

// Scalar istället för Swagger UI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://localhost:7201/scalar",
            UseShellExecute = true
        });
    });
}



// Demo 1
app.MapGet("/api/hello", () =>
    Results.Ok(new { message = "Hello!", timestamp = DateTime.UtcNow }));

// Demo 2
app.MapPost("/api/order", (OrderRequest order, TelemetryClient telemetry) =>
{
    telemetry.TrackEvent(
        "OrderPlaced",
        new Dictionary<string, string>
        {
            ["ProductId"] = order.ProductId,
            ["CustomerId"] = order.CustomerId
        },
        new Dictionary<string, double>
        {
            ["Amount"] = order.Amount,
            ["Quantity"] = order.Quantity
        });

    return Results.Ok(new { orderId = Guid.NewGuid() });
});

// Demo 3
app.MapGet("/api/metrics/queue", (TelemetryClient telemetry) =>
{
    telemetry.TrackMetric("QueueLength", Random.Shared.Next(0, 100));
    return Results.Ok();
});

// Demo 4
app.MapGet("/api/weather", async (HttpClient http, TelemetryClient telemetry) =>
{
    var res = await http.GetAsync("https://api.open-meteo.com/v1/forecast?latitude=60&longitude=18&current_weather=true");
    telemetry.TrackEvent("WeatherFetched");
    return Results.Ok();
});

// Demo 5
app.MapGet("/api/products/{id:int}", async (int id, TelemetryClient telemetry) =>
{
    using var op = telemetry.StartOperation<DependencyTelemetry>("ProductDatabase");
    await Task.Delay(100);
    op.Telemetry.Success = true;

    return Results.Ok(new { id, name = $"Product {id}" });
});

// Demo 6
app.MapGet("/api/error", (TelemetryClient telemetry) =>
{
    try
    {
        throw new InvalidOperationException("Demo exception");
    }
    catch (Exception ex)
    {
        telemetry.TrackException(ex);
        return Results.Problem(ex.Message);
    }
});

// Demo 7
app.MapGet("/api/slow/{ms:int}", async (int ms, TelemetryClient telemetry) =>
{
    using var op = telemetry.StartOperation<RequestTelemetry>("SlowOperation");
    await Task.Delay(ms);
    return Results.Ok();
});

// Demo 8
app.MapGet("/api/process/{items:int}", (int items, TelemetryClient telemetry) =>
{
    telemetry.TrackTrace($"Processing {items} items", SeverityLevel.Information);
    return Results.Ok();
});

// Demo 9
app.MapGet("/api/health", (TelemetryClient telemetry) =>
{
    telemetry.TrackAvailability(new AvailabilityTelemetry
    {
        Name = "HealthCheck",
        Success = true,
        Duration = TimeSpan.FromMilliseconds(50)
    });

    return Results.Ok("Healthy");
});

// Demo 10
app.MapGet("/api/pageview/{page}", (string page, TelemetryClient telemetry) =>
{
    telemetry.TrackPageView(page);
    return Results.Ok();
});

//Live Metric endpoint
app.MapGet("/api/live-metrics-stress", (TelemetryClient telemetry) =>
{
    // Kraftigare CPU (single request räcker)
    var sw = Stopwatch.StartNew();
    while (sw.Elapsed < TimeSpan.FromSeconds(5))
    {
        for (int i = 0; i < 1_000_000; i++)
        {
            Math.Pow(i, 1.5);
        }


    }

    for (int i = 0; i < 50; i++)
    {
        telemetry.TrackException(
            new InvalidOperationException($"Tracked exception {i}")
        );
    }

    // Unhandled exception (räknas i Exception rate)
    throw new ApplicationException("Final unhandled exception");

   

});

app.Run();

public record OrderRequest(string ProductId, string CustomerId, double Amount, int Quantity);
