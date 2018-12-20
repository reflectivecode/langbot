
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LangBot.Web.Slack
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SlackMessageActionTypes
    {
        [EnumMember(Value = "button")] Button,
        [EnumMember(Value = "select")] Select,
    }
}
