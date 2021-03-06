
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LangBot.Web.Slack
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SlackDialogElementTypes
    {
        [EnumMember(Value = "text")] Text,
        [EnumMember(Value = "textarea")] Textarea,
        [EnumMember(Value = "select")] Select,
    }
}
