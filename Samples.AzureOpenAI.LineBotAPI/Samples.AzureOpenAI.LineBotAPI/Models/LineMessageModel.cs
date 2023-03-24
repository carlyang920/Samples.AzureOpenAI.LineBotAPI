namespace Samples.AzureOpenAI.LineBotAPI.Models
{
    public class LineMessage
    {
        public string destination { get; set; }
        public List<Event> events { get; set; }
    }
    public class DeliveryContext
    {
        public bool isRedelivery { get; set; }
    }

    public class Event
    {
        public string type { get; set; }
        public MessageWithID message { get; set; }
        public string webhookEventId { get; set; }
        public DeliveryContext deliveryContext { get; set; }
        public long timestamp { get; set; }
        public Source source { get; set; }
        public string replyToken { get; set; }
        public string mode { get; set; }
    }

    public class BaseMessage
    {
        public string type { get; set; }
        public string text { get; set; }
    }

    public class MessageWithID : BaseMessage
    {
        public string id { get; set; }
    }

    public class Source
    {
        public string type { get; set; }
        public string? userId { get; set; }
        public string? groupId { get; set; }
    }

    public class ReplyMessage
    {
        public string replyToken { get; set; }
        public List<BaseMessage> messages { get; set; }
        public bool? notificationDisabled { get; set; } = false;
    }

    public class LinePushMessage
    {
        public string to { get; set; }
        public List<BaseMessage> messages { get; set; }
        public bool? notificationDisabled { get; set; } = false;
    }
}
