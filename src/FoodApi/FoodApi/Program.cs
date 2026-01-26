using Prometheus;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(_ => { });
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpLogging();

// Custom metrics
var ordersCreated = Metrics.CreateCounter(
    "foodapi_orders_created_total",
    "Total number of orders created");

    

var requestCount = Metrics.CreateCounter(
    "foodapi_http_requests_total",
    "HTTP requests total",
    new CounterConfiguration { LabelNames = new[] { "method", "endpoint", "code" } });

var requestDuration = Metrics.CreateHistogram(
    "foodapi_http_request_duration_seconds",
    "HTTP request duration in seconds",
    new HistogramConfiguration { Buckets = Histogram.ExponentialBuckets(0.005, 2, 12) });

// Metrics endpoint + default .NET metrics
app.UseMetricServer("/metrics");

// Middleware to record per-request metrics
app.Use(async (ctx, next) =>
{
    var sw = System.Diagnostics.Stopwatch.StartNew();
    try
    {
        await next();
    }
    finally
    {
        sw.Stop();
        var endpoint = ctx.GetEndpoint()?.DisplayName ?? "unknown";
        var code = ctx.Response.StatusCode.ToString();

        requestCount.WithLabels(ctx.Request.Method, endpoint, code).Inc();
        requestDuration.Observe(sw.Elapsed.TotalSeconds);
    }
});

app.MapGet("/health", () => Results.Ok(new { ok = true }))
   .WithName("health");

app.MapPost("/orders", () =>
{
    ordersCreated.Inc();
    return Results.Accepted(value: new { status = "created" });
}).WithName("create_order");

app.Run();
