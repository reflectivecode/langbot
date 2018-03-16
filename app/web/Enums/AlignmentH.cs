using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LangBot.Web
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AlignmentH
    {
        Left,
        Center,
        Right,
    }
}
