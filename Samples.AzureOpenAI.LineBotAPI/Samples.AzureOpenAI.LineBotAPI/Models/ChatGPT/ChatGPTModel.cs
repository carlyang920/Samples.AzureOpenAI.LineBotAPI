namespace Samples.AzureOpenAI.LineBotAPI.Models.ChatGPT
{
    public class RequestChatGPTModel
    {
        public List<Message> messages { get; set; }
        public double temperature { get; set; } = 0.7;
        public double top_p { get; set; } = 0.95;
        public int frequency_penalty { get; set; } = 0;
        public int presence_penalty { get; set; } = 0;
        public int max_tokens { get; set; } = 800;
        public object? stop { get; set; } = null;
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class ResponseChatGPTModel
    {
        public string id { get; set; }
        public string @object { get; set; }
        public int created { get; set; }
        public string model { get; set; }
        public List<Choice> choices { get; set; }
        public Usage usage { get; set; }
    }

    public class Choice
    {
        public int index { get; set; }
        public object finish_reason { get; set; }
        public Message message { get; set; }
    }

    public class Usage
    {
        public int completion_tokens { get; set; }
        public int prompt_tokens { get; set; }
        public int total_tokens { get; set; }
    }
}
