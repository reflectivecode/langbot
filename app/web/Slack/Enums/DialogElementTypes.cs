
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LangBot.Web.Slack
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DialogElementTypes
    {
        Text,
        Textarea,
        Select,
    }
}
