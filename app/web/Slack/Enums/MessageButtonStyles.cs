
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LangBot.Web.Slack
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MessageButtonStyles
    {
        [EnumMember(Value = "default")] Default,
        [EnumMember(Value = "primary")] Primary,
        [EnumMember(Value = "danger")] Danger,
    }
}
