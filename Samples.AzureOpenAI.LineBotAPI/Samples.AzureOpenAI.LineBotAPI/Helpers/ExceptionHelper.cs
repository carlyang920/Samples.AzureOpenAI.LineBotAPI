using Newtonsoft.Json;
using Samples.AzureOpenAI.LineBotAPI.Extensions;

namespace Samples.AzureOpenAI.LineBotAPI.Helpers
{
    public static class ExceptionHelper
    {
        public static void GetExInfo(
            Exception e,
            Dictionary<string, string>? extraInfo
        )
        {
            var innerKeyWord = "";

            while (null != e)
            {
                extraInfo ??= new Dictionary<string, string>();

                extraInfo.AddValueIfKeyNotExist($"Custom-{innerKeyWord}Exception", $"{e}");
                extraInfo.AddValueIfKeyNotExist($"Custom-{innerKeyWord}ClassName", $"{e.GetType().FullName}");
                extraInfo.AddValueIfKeyNotExist($"Custom-{innerKeyWord}HResult", $"{e.HResult}");
                extraInfo.AddValueIfKeyNotExist($"Custom-{innerKeyWord}HelpLink", $"{e.HelpLink}");
                extraInfo.AddValueIfKeyNotExist($"Custom-{innerKeyWord}Data", JsonConvert.SerializeObject(e.Data, Formatting.None));
                extraInfo.AddValueIfKeyNotExist($"Custom-{innerKeyWord}Source", $"{e.Source}");
                extraInfo.AddValueIfKeyNotExist($"Custom-{innerKeyWord}Message", e.Message);
                extraInfo.AddValueIfKeyNotExist($"Custom-{innerKeyWord}StackTrace", $"{e.StackTrace}");

                e = e.InnerException;
                innerKeyWord = $"{innerKeyWord}Inner";
            }
        }
    }
}
