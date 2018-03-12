
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LangBot.Web.Slack
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SlackMessageResponseTypes
    {
        [EnumMember(Value = "ephemeral")] Ephemeral,
        [EnumMember(Value = "in_channel")] InChannel,
    }
}
