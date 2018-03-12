using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LangBot.Web.Slack
{
    public abstract class SlackBaseReadConverter<T> : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;
        public override bool CanConvert(Type objectType) => objectType == typeof(T);
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new InvalidOperationException("Use default serialization.");

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var type = GetObjectType(jObject);
            return serializer.Deserialize(jObject.CreateReader(), type);
        }

        abstract protected Type GetObjectType(JObject jObject);
    }
}
