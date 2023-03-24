using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Samples.AzureOpenAI.LineBotAPI.Models;
using Samples.AzureOpenAI.LineBotAPI.Services;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var config = builder.Configuration.Get<ConfigModel>();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        // Use the default property (Pascal) casing
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        options.SerializerSettings.Formatting = Formatting.None;
    });

builder.Services.AddApplicationInsightsTelemetry();
builder.Logging.AddApplicationInsights(
    configureTelemetryConfiguration: (c) => c.ConnectionString = config.ApplicationInsights.ConnectionString,
    configureApplicationInsightsLoggerOptions: (_) => { }
);
// Capture all log-level entries from Program
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(
    typeof(Program).FullName, LogLevel.Trace);
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<MonitorService>();
builder.Services.AddHttpClient<HttpClientService>()
    .AddPolicyHandler((p, _) => GetRetryPolicy(p.GetRequiredService<MonitorService>()));
builder.Services.AddScoped<AzureOpenAIService>();
builder.Services.AddScoped<LineBotService>();
builder.Services.AddScoped<LineNotifyService>();
builder.Services.AddSingleton<MemoryCacheService>();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

//To prevent GET https://root/ 404 Error on Azure Web App(AlwaysOn)
app.MapHealthChecks("/");
app.MapControllers();

app.Run();

IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(MonitorService monitorService)
{
    async Task OnRetryAsync(
        DelegateResult<HttpResponseMessage> result,
        TimeSpan timespan,
        int retryCount,
        Context ctx
    )
    {
        await Task.Run(() =>
        {
            monitorService.ThrowMessageTrace(
                $"FunctionName: [{nameof(Program)}] {nameof(GetRetryPolicy)}()",
                "API Retry Information",
                "Retry Information",
                new Dictionary<string, string>()
                {
                    { "Result", $"{JsonConvert.SerializeObject(result, Formatting.None)}" },
                    { "Timespan", $"{timespan}" },
                    { "CurrentTime", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}" },
                    { "RetryCount", $"{retryCount}" },
                    { "Context", $"{JsonConvert.SerializeObject(ctx, Formatting.None)}" }
                }
            );
        });
    }

    var jitter = RandomNumberGenerator.GetInt32(5, 30);

    return HttpPolicyExtensions
        //For HttpRequestException/5XX/408 errors
        .HandleTransientHttpError()
        //IO Exception
        .Or<IOException>()
        //For 404 NoFound
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        //Retry N times and latency M seconds
        .WaitAndRetryAsync(
            2,
            _ => TimeSpan.FromSeconds(30) + TimeSpan.FromSeconds(jitter),
            OnRetryAsync
        );
}