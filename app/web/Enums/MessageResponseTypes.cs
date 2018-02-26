
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LangBot.Web.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MessageResponseTypes
    {
        [EnumMember(Value = "ephemeral")] Ephemeral,
        [EnumMember(Value = "in_channel")] InChannel,
    }
}
