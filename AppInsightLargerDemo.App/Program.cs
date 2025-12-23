using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

var builder = WebApplication.CreateBuilder(args);

// Lägg till Application Insights
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["Your Connection String here"];
});

//https://localhost:7201/swagger/index.html
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Registrera TelemetryClient som singleton för custom tracking
builder.Services.AddSingleton<TelemetryClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Demo 1: Enkel request som automatiskt trackas
app.MapGet("/api/hello", () =>
{
    return Results.Ok(new { message = "Hello from Application Insights Demo!", timestamp = DateTime.UtcNow });
})
.WithName("GetHello")
.WithOpenApi();

// Demo 2: Custom Event Tracking - Business metrics
app.MapPost("/api/order", (OrderRequest order, TelemetryClient telemetry) =>
{
    // Tracka custom business event
    var properties = new Dictionary<string, string>
    {
        { "ProductId", order.ProductId },
        { "CustomerId", order.CustomerId },
        { "Category", "Electronics" }
    };

    var metrics = new Dictionary<string, double>
    {
        { "Amount", order.Amount },
        { "Quantity", order.Quantity }
    };

    telemetry.TrackEvent("OrderPlaced", properties, metrics);

    return Results.Ok(new { orderId = Guid.NewGuid(), status = "Created" });
})
.WithName("CreateOrder")
.WithOpenApi();

// Demo 3: Custom Metrics - Performance counter
app.MapGet("/api/metrics/queue", (TelemetryClient telemetry) =>
{
    // Simulera queue length metric
    var queueLength = Random.Shared.Next(0, 100);
    telemetry.TrackMetric("QueueLength", queueLength);

    // Metric med additional properties
    var metric = new MetricTelemetry("ProcessingTime", Random.Shared.Next(100, 1000));
    metric.Properties.Add("Operation", "DataProcessing");
    telemetry.TrackMetric(metric);

    return Results.Ok(new { queueLength, message = "Metrics tracked!" });
})
.WithName("TrackMetrics")
.WithOpenApi();

// Demo 4: Dependency Tracking - Externa anrop
app.MapGet("/api/weather/{city}", async (string city, HttpClient httpClient, TelemetryClient telemetry) =>
{
    try
    {
        // HTTP dependency trackas automatiskt
        var response = await httpClient.GetAsync($"https://api.open-meteo.com/v1/forecast?latitude=60.67&longitude=17.14&current_weather=true");

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();
            telemetry.TrackEvent("WeatherFetched", new Dictionary<string, string> { { "City", city } });
            return Results.Ok(new { city, data = "Weather data retrieved", status = "success" });
        }

        return Results.Problem("Failed to fetch weather data");
    }
    catch (Exception ex)
    {
        telemetry.TrackException(ex);
        return Results.Problem("Error fetching weather");
    }
})
.WithName("GetWeather")
.WithOpenApi();

// Demo 5: Custom Dependency Tracking - Databas-anrop (simulerat)
app.MapGet("/api/products/{id}", async (int id, TelemetryClient telemetry) =>
{
    var dependency = telemetry.StartOperation<DependencyTelemetry>("ProductDatabase");
    dependency.Telemetry.Type = "SQL";
    dependency.Telemetry.Data = $"SELECT * FROM Products WHERE Id = {id}";

    try
    {
        // Simulera databas-anrop
        await Task.Delay(Random.Shared.Next(50, 200));

        var product = new { id, name = $"Product {id}", price = Random.Shared.Next(10, 1000) };

        dependency.Telemetry.Success = true;
        dependency.Telemetry.ResultCode = "200";

        return Results.Ok(product);
    }
    catch (Exception ex)
    {
        dependency.Telemetry.Success = false;
        telemetry.TrackException(ex);
        return Results.Problem("Database error");
    }
    finally
    {
        telemetry.StopOperation(dependency);
    }
})
.WithName("GetProduct")
.WithOpenApi();

// Demo 6: Exception Tracking
app.MapGet("/api/error/{type}", (string type, TelemetryClient telemetry) =>
{
    try
    {
        switch (type.ToLower())
        {
            case "null":
                string? nullString = null;
                return Results.Ok(nullString!.Length); // Kastar NullReferenceException

            case "divide":
                var result = 10 / int.Parse("0"); // Kastar DivideByZeroException
                return Results.Ok(result);

            case "custom":
                throw new InvalidOperationException("Detta är en custom exception för demo!");

            default:
                return Results.BadRequest("Unknown error type. Try: null, divide, or custom");
        }
    }
    catch (Exception ex)
    {
        // Tracka exception med extra context
        var exceptionTelemetry = new ExceptionTelemetry(ex);
        exceptionTelemetry.Properties.Add("ErrorType", type);
        exceptionTelemetry.Properties.Add("UserId", "demo-user-123");
        exceptionTelemetry.SeverityLevel = SeverityLevel.Error;

        telemetry.TrackException(exceptionTelemetry);

        return Results.Problem($"Exception tracked: {ex.Message}");
    }
})
.WithName("TriggerError")
.WithOpenApi();

// Demo 7: Performance tracking med olika response times
app.MapGet("/api/slow/{delay}", async (int delay, TelemetryClient telemetry) =>
{
    var operation = telemetry.StartOperation<RequestTelemetry>("SlowOperation");
    operation.Telemetry.Properties.Add("RequestedDelay", delay.ToString());

    try
    {
        // Simulera långsam operation
        await Task.Delay(delay);

        operation.Telemetry.Success = true;
        operation.Telemetry.ResponseCode = "200";

        return Results.Ok(new { message = $"Completed after {delay}ms", duration = delay });
    }
    finally
    {
        telemetry.StopOperation(operation);
    }
})
.WithName("SlowEndpoint")
.WithOpenApi();

// Demo 8: Trace/Logging
app.MapGet("/api/process/{items}", (int items, TelemetryClient telemetry, ILogger<Program> logger) =>
{
    logger.LogInformation("Processing started with {ItemCount} items", items);
    telemetry.TrackTrace($"Processing {items} items", SeverityLevel.Information);

    for (int i = 0; i < items; i++)
    {
        if (i % 10 == 0)
        {
            telemetry.TrackTrace($"Processed {i}/{items} items",
                SeverityLevel.Verbose,
                new Dictionary<string, string> { { "Progress", $"{(i * 100 / items)}%" } });
        }
    }

    logger.LogInformation("Processing completed");
    telemetry.TrackTrace($"Completed processing {items} items", SeverityLevel.Information);

    return Results.Ok(new { processed = items, status = "completed" });
})
.WithName("ProcessItems")
.WithOpenApi();

// Demo 9: Availability tracking
app.MapGet("/api/health", (TelemetryClient telemetry) =>
{
    var availability = new AvailabilityTelemetry
    {
        Name = "Health Check",
        RunLocation = "Azure-SwedenCentral",
        Success = true,
        Duration = TimeSpan.FromMilliseconds(Random.Shared.Next(10, 100))
    };

    availability.Properties.Add("HealthStatus", "Healthy");
    telemetry.TrackAvailability(availability);

    return Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
})
.WithName("HealthCheck")
.WithOpenApi();

// Demo 10: Page View tracking (simulerat för API)
app.MapGet("/api/pageview/{pageName}", (string pageName, TelemetryClient telemetry) =>
{
    var pageView = new PageViewTelemetry(pageName)
    {
        Url = new Uri($"https://demo.example.com/{pageName}"),
        Duration = TimeSpan.FromMilliseconds(Random.Shared.Next(500, 2000))
    };

    pageView.Properties.Add("Browser", "Chrome");
    pageView.Properties.Add("DeviceType", "Desktop");

    telemetry.TrackPageView(pageView);

    return Results.Ok(new { page = pageName, tracked = true });
})
.WithName("TrackPageView")
.WithOpenApi();

app.Run();

// Request models
public record OrderRequest(string ProductId, string CustomerId, double Amount, int Quantity);