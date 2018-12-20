using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LangBot.Web
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AlignmentV
    {
        Top,
        Center,
        Bottom,
    }
}
