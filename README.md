# Samples.AzureOpenAI.LineBotAPI
This project is made for processing the message passing between Azure OpenAI Service and Line Messaging API. It's write by ASP.NET Core 6.0 and C# but only can process message text. If there're some Line Bot events like image, video and broadcast...etc, this project is not useful.

## Environment
Please set environment variable: `ASPNETCORE_ENVIRONMENT = Development`  
If you want to use other environment or custom it, you must change your appsettings.[ENV].json file name.

## appsettings.Development.json
Before run this project, please complete all configuration first. 

```json
{
  "ApplicationInsights": {
    "ConnectionString": "[Your Application Insights connection string]"
  },
  "AzureOpenAIConfig": {
    "Address": "[Your Azure OpenAI Service address]",
    "APIKey": "[Your Azure OpenAI Service API Key]",
    "RecentN": 20,
    "MaxTokens": 20000,
    "Temperature": 0.13,
    "TopP": 0.95,
    "FrequencyPenalty": 0,
    "PresencePenalty": 0,
    "Stream": false
  },
  "LineBotConfig": {
    "AccessToken": "[Your Line Messaging API Access Token]",
    "Address": "https://api.line.me/v2/bot/message/",
    "ReplyAction": "reply",
    "PushAction": "push"
  },
  "LineNotifyConfig": {
    "AccessToken": "[Your Line Notify Access Token]",
    "Address": "https://notify-api.line.me/api/notify"
  },
  "Users": [
    {
      "Id": "[The user ID which is requested from client and used in PUSH API to identify specific user]",
      "LineId": "[Your Line user Id]"
    }
  ]
}
```
### API URLs
1. The line bot api URL is as below. You must put it to Line Messaging API's Webhook URL setting. Then Line Bot will know where is it to send message.

```
https://[Your Web App Name].azurewebsites.net/api/LineBot/Message
```

2. This API is made for manually send PUSH message to specific user.
```
https://[Your Web App Name].azurewebsites.net/api/LineBot/push
```

In order to hide Line user ID, we use another user ID which is easy to understand in settings. The client will pass it to our API then API will get the mapping Line user ID and use it in `Line Message API` payload.

```json
"Users": [
  {
    "Id": "[The user ID which is requested from client and used in PUSH API to identify specific user]",
    "LineId": "[Your Line user Id]"
  }
]
```

3. There're maximum 500 messages per month of Line PUSH message. So we also create a Line notify API to avoid this restriction.

```
https://[Your Web App Name].azurewebsites.net/api/LineBot/notify
```

### Application Insights
You have to get the Application Insights' connection string from Azure portal, not only the InstrumentationKey. This is an efficient way to log application errors.

```json
"ApplicationInsights": {
  "ConnectionString": "[Your Application Insights connection string]"
},
```