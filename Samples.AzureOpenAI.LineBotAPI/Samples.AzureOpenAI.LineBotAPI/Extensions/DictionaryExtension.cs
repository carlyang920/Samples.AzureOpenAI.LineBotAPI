namespace Samples.AzureOpenAI.LineBotAPI.Extensions
{
    public static class DictionaryExtension
    {
        public static Dictionary<string, string> AddValueIfKeyNotExist(this Dictionary<string, string> dic, string key, string value)
        {
            if (!dic.ContainsKey(key)) dic.Add(key, value);
            return dic;
        }
    }
}
