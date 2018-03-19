
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LangBot.Web.Slack
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SlackDialogElementSubTypes
    {
        [EnumMember(Value = "email")] Email,
        [EnumMember(Value = "number")] Number,
        [EnumMember(Value = "tel")] Tel,
        [EnumMember(Value = "url")] Url,
    }
}
