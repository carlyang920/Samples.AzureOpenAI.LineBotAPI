using Newtonsoft.Json;
using Samples.AzureOpenAI.LineBotAPI.Extensions;
using Samples.AzureOpenAI.LineBotAPI.Models;

namespace Samples.AzureOpenAI.LineBotAPI.Services
{
    public class LineBotService
    {
        private readonly ConfigModel _config;
        private readonly HttpClientService _httpClientService;

        public LineBotService(
            IConfiguration config,
            HttpClientService httpClientService
        )
        {
            _config = config.Get<ConfigModel>();
            _httpClientService = httpClientService;
        }

        private async Task<TResponse?> RequestAsync<TResponse>(
            HttpMethod method, string address, string? body)
        {
            var headers = new Dictionary<string, string>();

            headers.AddValueIfKeyNotExist("Authorization", $"Bearer {_config.LineBotConfig.AccessToken}");

            var response = await _httpClientService.SendAsync(
                method,
                address,
                "application/json",
                null,
                body,
                headers
            );

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public async Task<TResponse?> ReplyAsync<TResponse>(
            HttpMethod method, string? body)
        {
            return await RequestAsync<TResponse>(
                method,
                $"{_config.LineBotConfig.Address}{_config.LineBotConfig.ReplyAction}",
                body
            );
        }

        public async Task<TResponse?> PushAsync<TResponse>(
            HttpMethod method, string? body)
        {
            return await RequestAsync<TResponse>(
                method,
                $"{_config.LineBotConfig.Address}{_config.LineBotConfig.PushAction}",
                body
            );
        }
    }
}