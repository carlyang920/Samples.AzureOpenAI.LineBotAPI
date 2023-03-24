using Microsoft.Extensions.Caching.Memory;

namespace Samples.AzureOpenAI.LineBotAPI.Services
{
    public class MemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        private int GetMinNumber(string prefix)
        {
            var count = 1;
            var result = 0;

            do
            {
                var key = $"{prefix}_{count}";

                _memoryCache.TryGetValue(
                    key,
                    out string cachedValue);

                if (!string.IsNullOrEmpty(cachedValue))
                {
                    result = count;
                    break;
                }

                if (100000 <= count && string.IsNullOrEmpty(cachedValue))
                    break;

                count++;
            } while (true);

            return result;
        }

        private int GetMaxNumber(string prefix)
        {
            var count = GetMinNumber(prefix);
            var result = 0;

            if (0 >= count) return count;

            do
            {
                var key = $"{prefix}_{count}";

                _memoryCache.TryGetValue(
                    key,
                    out string cachedValue);

                if (!string.IsNullOrEmpty(cachedValue))
                    result = count;
                else
                    break;

                count++;
            } while (true);

            return result;
        }

        public List<string> GetValues(string prefix, int recentN = 10)
        {
            var min = GetMinNumber(prefix);
            var max = GetMaxNumber(prefix);
            var cacheList = new List<string>();

            for (var i = (max - recentN > min ? max - recentN : min); i <= max; i++)
            {
                var key = $"{prefix}_{i}";

                _memoryCache.TryGetValue(
                    key,
                    out string? cachedValue);

                cacheList.Add(cachedValue!);
            }

            return cacheList;
        }

        public void SetValue(string prefix, string val)
        {
            var max = GetMaxNumber(prefix);

            _memoryCache.Set($"{prefix}_{max + 1}", val, TimeSpan.FromMinutes(10));
        }
    }
}