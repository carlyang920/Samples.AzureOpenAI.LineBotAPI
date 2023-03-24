namespace Samples.AzureOpenAI.LineBotAPI.Models
{
    public class ConfigModel
    {
        public ApplicationInsights ApplicationInsights { get; set; } = new();
        public AzureOpenAIConfig AzureOpenAiConfig { get; set; } = new();
        public LineBotConfig LineBotConfig { get; set; } = new();
        public LineNotifyConfig LineNotifyConfig { get; set; } = new();
        public List<User> Users { get; set; } = new();
    }

    public class ApplicationInsights
    {
        public string ConnectionString { get; set; } = string.Empty;

        public string InstrumentationKey => ConnectionString.Split(';').ToList().FirstOrDefault(c => c.Contains("InstrumentationKey"))?.Split('=')[1] ?? string.Empty;
    }

    public class AzureOpenAIConfig
    {
        public string? Address { get; set; }
        public string? APIKey { get; set; }
        public int RecentN { get; set; } = 10;
        public int MaxTokens { get; set; } = 800;
    }

    public class LineBotConfig
    {
        public string AccessToken { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ReplyAction { get; set; } = string.Empty;
        public string PushAction { get; set; } = string.Empty;
    }

    public class LineNotifyConfig
    {
        public string AccessToken { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string LineId { get; set; } = string.Empty;
    }
}
