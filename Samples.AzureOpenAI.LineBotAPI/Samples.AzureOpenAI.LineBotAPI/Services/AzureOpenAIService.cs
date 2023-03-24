using Newtonsoft.Json;
using Samples.AzureOpenAI.LineBotAPI.Extensions;
using Samples.AzureOpenAI.LineBotAPI.Models;

namespace Samples.AzureOpenAI.LineBotAPI.Services
{
    public class AzureOpenAIService
    {
        private readonly ConfigModel _config;
        private readonly HttpClientService _httpClientService;

        public AzureOpenAIService(
            IConfiguration config,
            HttpClientService httpClientService
        )
        {
            _config = config.Get<ConfigModel>();
            _httpClientService = httpClientService;
        }

        public async Task<TResponse?> RequestAsync<TResponse>(
            HttpMethod method, string? body)
        {
            var headers = new Dictionary<string, string>();

            headers.AddValueIfKeyNotExist("api-key", _config.AzureOpenAiConfig.APIKey ?? string.Empty);

            var address = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AzureOpenAIAddress"))
                ? Environment.GetEnvironmentVariable("AzureOpenAIAddress")
                : _config.AzureOpenAiConfig.Address;

            var response = await _httpClientService.SendAsync(
                method,
                address!,
                "application/json",
                null,
                body,
                headers
            );

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(content);
        }
    }
}