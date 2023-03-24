using System.Text;
using Samples.AzureOpenAI.LineBotAPI.Extensions;

namespace Samples.AzureOpenAI.LineBotAPI.Services
{
    public class HttpClientService
    {
        private readonly HttpClient _httpClient;

        public HttpClientService(
            HttpClient httpClient
        )
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SendAsync(
            HttpMethod method, string address, string contentType, string? queryString, string? body, IDictionary<string, string>? headers
        )
        {
            if (null != headers)
                foreach (var d in headers)
                {
                    _httpClient.DefaultRequestHeaders.AddIfNotExists(d.Key, d.Value);
                }

            StringContent? payload = null;
            if (!string.IsNullOrEmpty(body))
            {
                payload = new StringContent(body, Encoding.UTF8, contentType);
            }

            var request = new HttpRequestMessage()
            {
                Method = method,
                RequestUri = new Uri($"{address}{queryString}"),
                Content = payload
            };

            var response = await _httpClient.SendAsync(request);

            return response;
        }
    }
}