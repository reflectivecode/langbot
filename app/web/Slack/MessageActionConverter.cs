using System;
using LangBot.Web.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LangBot.Web.Slack
{
    public class MessageActionConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;
        public override bool CanConvert(Type objectType) => objectType == typeof(IMessageAction);
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new InvalidOperationException("Use default serialization.");

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var type = jsonObject["type"].Value<string>();
            var action = CreateActionFromType(type);
            serializer.Populate(jsonObject.CreateReader(), action);
            return action;
        }

        private IMessageAction CreateActionFromType(string type)
        {
            switch (Enum.Parse<MessageActionTypes>(type, true))
            {
                case MessageActionTypes.Button: return new MessageButton();
                case MessageActionTypes.Select: return new MessageSelect();
                default: throw new SlackException($"Unexpected action type: {type}");
            }
        }
    }
}
