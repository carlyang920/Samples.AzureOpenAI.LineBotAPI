using System.Net.Http.Headers;

namespace Samples.AzureOpenAI.LineBotAPI.Extensions
{
    public static class HttpHeadersExtension
    {
        public static HttpRequestHeaders AddAlways(this HttpRequestHeaders headers, string key, string value)
        {
            if (headers.Contains(key))
                headers.Remove(key);

            headers.Add(key, value);

            return headers;
        }

        public static HttpRequestHeaders AddIfNotExists(this HttpRequestHeaders headers, string key, string value)
        {
            if (!headers.Contains(key))
                headers.Add(key, value);

            return headers;
        }

        public static HttpRequestHeaders RemoveIfExists(this HttpRequestHeaders headers, string key)
        {
            if (headers.Contains(key))
                headers.Remove(key);

            return headers;
        }
    }
}
