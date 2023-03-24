using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Samples.AzureOpenAI.LineBotAPI.Extensions;
using Samples.AzureOpenAI.LineBotAPI.Helpers;
using Samples.AzureOpenAI.LineBotAPI.Models;

namespace Samples.AzureOpenAI.LineBotAPI.Services
{
    public class MonitorService
    {
        private readonly TelemetryClient _telemetry;

        public MonitorService(
            IConfiguration config
            )
        {
            var config1 = config.Get<ConfigModel>();

            _telemetry = new TelemetryClient(TelemetryConfiguration.CreateDefault())
            {
                InstrumentationKey = config1.ApplicationInsights.InstrumentationKey
            };
        }

        public void ThrowMessageTrace(string functionName, string title, string message, Dictionary<string, string>? extraInfo)
        {
            extraInfo ??= new Dictionary<string, string>();
            extraInfo.AddValueIfKeyNotExist("CurrentTime", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            extraInfo.AddValueIfKeyNotExist("FunctionName", $"{functionName}");
            extraInfo.AddValueIfKeyNotExist("Title", $"{title}");
            extraInfo.AddValueIfKeyNotExist("Message", $"{message}");

            _telemetry.TrackTrace($"{message}", extraInfo);
        }

        public void ThrowException(string functionName, Exception ex, Dictionary<string, string>? extraInfo)
        {
            extraInfo ??= new Dictionary<string, string>();
            extraInfo.AddValueIfKeyNotExist("CurrentTime", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            extraInfo.AddValueIfKeyNotExist("FunctionName", $"{functionName}");

            ExceptionHelper.GetExInfo(ex, extraInfo);

            _telemetry.TrackException(ex, extraInfo);
        }
    }
}
