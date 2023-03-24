namespace Samples.AzureOpenAI.LineBotAPI.Models
{
    public class RequestMessageModel
    {
        public string UserId { get; set; }
        public List<MessageContent> messages { get; set; }
    }

    public class MessageContent
    {
        public string Content { get; set; }
    }
}
