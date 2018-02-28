using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LangBot.Web.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AlignmentV
    {
        Top,
        Center,
        Bottom,
    }
}
