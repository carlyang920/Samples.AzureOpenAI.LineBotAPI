using Newtonsoft.Json;
using Samples.AzureOpenAI.LineBotAPI.Extensions;
using Samples.AzureOpenAI.LineBotAPI.Models;

namespace Samples.AzureOpenAI.LineBotAPI.Services
{
    public class LineNotifyService
    {
        private readonly ConfigModel _config;
        private readonly HttpClientService _httpClientService;

        public LineNotifyService(
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

            headers.AddValueIfKeyNotExist("Authorization", $"Bearer {_config.LineNotifyConfig.AccessToken}");

            var response = await _httpClientService.SendAsync(
                method,
                address,
                "application/x-www-form-urlencoded",
                null,
                body,
                headers
            );

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public async Task<TResponse?> SendAsync<TResponse>(
            HttpMethod method, string? body)
        {
            return await RequestAsync<TResponse>(
                method,
                $"{_config.LineNotifyConfig.Address}",
                body
            );
        }
    }
}