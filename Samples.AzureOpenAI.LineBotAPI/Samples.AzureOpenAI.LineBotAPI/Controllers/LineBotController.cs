using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Samples.AzureOpenAI.LineBotAPI.Models;
using Samples.AzureOpenAI.LineBotAPI.Models.ChatGPT;
using Samples.AzureOpenAI.LineBotAPI.Services;

namespace Samples.AzureOpenAI.LineBotAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LineBotController : Controller
    {
        private readonly ConfigModel _config;
        private readonly MonitorService _monitorService;
        private readonly AzureOpenAIService _openAIService;
        private readonly LineBotService _lineBotService;
        private readonly LineNotifyService _lineNotifyService;
        private readonly MemoryCacheService _cacheService;

        public LineBotController(
            IConfiguration config,
            MonitorService monitorService,
            AzureOpenAIService openAIService,
            LineBotService lineBotService,
            LineNotifyService lineNotifyService,
            MemoryCacheService cacheService
        )
        {
            _config = config.Get<ConfigModel>();
            _monitorService = monitorService;
            _openAIService = openAIService;
            _lineBotService = lineBotService;
            _lineNotifyService = lineNotifyService;
            _cacheService = cacheService;
        }

        [HttpPost("Message")]
        public IActionResult SendMessage(LineMessage request)
        {
            try
            {
                _monitorService.ThrowMessageTrace(
                    $"{nameof(LineBotController)} {nameof(SendMessage)}()",
                    "Run API: Message",
                    "Request from Bot Message",
                    new Dictionary<string, string>()
                    {
                        {
                            "Request", JsonConvert.SerializeObject(request)
                        }
                    }
                );

                request.events.ForEach(e =>
                {
                    var replyToken = e.replyToken;
                    var userId = e.source.userId;

                    //Only to process text content temporarily
                    if (!e.type.Equals("message", StringComparison.CurrentCultureIgnoreCase)
                        || !e.message.type.Equals("text", StringComparison.OrdinalIgnoreCase))
                    {
                        _lineBotService.ReplyAsync<dynamic>(
                                HttpMethod.Post,
                                JsonConvert.SerializeObject(new ReplyMessage()
                                {
                                    replyToken = replyToken,
                                    messages = new List<BaseMessage>
                                        {new() {type = "text", text = "Sorry, I process text message only."}},
                                    notificationDisabled = false
                                })
                            )
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();

                        return;
                    }

                    //Save current message first
                    _cacheService.SetValue(userId!, e.message.text);

                    var recentN = !string.IsNullOrEmpty($"{Environment.GetEnvironmentVariable("RecentN")}")
                        ? int.Parse($"{Environment.GetEnvironmentVariable("RecentN")}")
                        : _config.AzureOpenAiConfig.RecentN;

                    var maxTokens = !string.IsNullOrEmpty($"{Environment.GetEnvironmentVariable("MaxTokens")}")
                        ? int.Parse($"{Environment.GetEnvironmentVariable("MaxTokens")}")
                        : _config.AzureOpenAiConfig.MaxTokens;

                    var answer = _openAIService.RequestAsync<ResponseChatGPTModel>(
                            HttpMethod.Post,
                            JsonConvert.SerializeObject(
                                new RequestChatGPTModel()
                                {
                                    //Get recent N messages
                                    messages = _cacheService.GetValues(userId!, recentN)
                                        .Select(m => new Message() {role = "user", content = m}).ToList(),
                                    max_tokens = maxTokens
                                }
                            )
                        )
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();

                    _monitorService.ThrowMessageTrace(
                        $"{nameof(LineBotController)} {nameof(SendMessage)}()",
                        "Run API: Message",
                        "Response from Azure OpenAI Service",
                        new Dictionary<string, string>()
                        {
                            {
                                "Response from Azure OpenAI Service", JsonConvert.SerializeObject(answer)
                            }
                        }
                    );

                    var replyMessages = new List<BaseMessage>();

                    answer?.choices.ForEach(c =>
                    {
                        replyMessages.Add(new BaseMessage
                        {
                            type = "text",
                            text = Regex.Replace(c.message.content, "<\\|[a-zA-Z0-9-_]+\\|>", string.Empty)
                        });
                    });

                    _lineBotService.ReplyAsync<dynamic>(
                            HttpMethod.Post,
                            JsonConvert.SerializeObject(new ReplyMessage()
                            {
                                replyToken = replyToken,
                                messages = replyMessages,
                                notificationDisabled = false
                            })
                        )
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                });
            }
            catch (Exception e)
            {
                _monitorService.ThrowException(
                    $"{nameof(LineBotController)} {nameof(SendMessage)}()",
                    e,
                    new Dictionary<string, string>()
                    {
                        {
                            "Request", JsonConvert.SerializeObject(request)
                        }
                    }
                );

                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("Push")]
        public IActionResult PushMessage(RequestMessageModel request)
        {
            try
            {
                _monitorService.ThrowMessageTrace(
                    $"{nameof(LineBotController)} {nameof(PushMessage)}()",
                    "Run API: Push",
                    "Push to Bot Message",
                    new Dictionary<string, string>()
                    {
                        {
                            "Request", JsonConvert.SerializeObject(request)
                        }
                    }
                );

                if (string.IsNullOrEmpty(request.UserId)
                    || !_config.Users.Exists(u => u.Id == request.UserId)) return BadRequest("No UserId");

                var pushMessage = new LinePushMessage()
                {
                    to = _config.Users.FirstOrDefault(u => u.Id == request.UserId)?.LineId ?? string.Empty,
                    notificationDisabled = false,
                    messages = new List<BaseMessage>()
                };

                request.messages.ForEach(m =>
                {
                    var msg = 1800 < m.Content.Length ? $"{m.Content[..1800]}...etc" : m.Content;
                    pushMessage.messages.Add(new BaseMessage {type = "text", text = msg });
                });

                _lineBotService.PushAsync<dynamic>(
                        HttpMethod.Post,
                        JsonConvert.SerializeObject(pushMessage)
                    )
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception e)
            {
                _monitorService.ThrowException(
                    $"{nameof(LineBotController)} {nameof(PushMessage)}()",
                    e,
                    new Dictionary<string, string>()
                    {
                        {
                            "Request", JsonConvert.SerializeObject(request)
                        }
                    }
                );

                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("Notify")]
        public IActionResult NotifyMessage(MessageContent request)
        {
            try
            {
                _monitorService.ThrowMessageTrace(
                    $"{nameof(LineBotController)} {nameof(NotifyMessage)}()",
                    "Run API: Notify",
                    "Notify message to Line Notify",
                    new Dictionary<string, string>()
                    {
                        {
                            "Request", JsonConvert.SerializeObject(request)
                        }
                    }
                );

                var msg = 900 < request.Content.Length ? $"{request.Content[..900]}...etc" : request.Content;
                var notifyMessage = $"message={msg}";

                _lineNotifyService.SendAsync<dynamic>(
                        HttpMethod.Post,
                        notifyMessage
                    )
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception e)
            {
                _monitorService.ThrowException(
                    $"{nameof(LineBotController)} {nameof(NotifyMessage)}()",
                    e,
                    new Dictionary<string, string>()
                    {
                        {
                            "Request", JsonConvert.SerializeObject(request)
                        }
                    }
                );

                return BadRequest();
            }

            return Ok();
        }
    }
}