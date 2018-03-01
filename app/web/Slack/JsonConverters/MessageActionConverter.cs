using System;
using Newtonsoft.Json.Linq;

namespace LangBot.Web.Slack
{
    public class MessageActionConverter : BaseReadConverter<IMessageAction>
    {
        protected override Type GetObjectType(JObject jObject)
        {
            var type = jObject["type"].Value<string>();
            switch (type)
            {
                case "button": return typeof(MessageButton);
                case "select": return typeof(MessageSelect);
                default: throw new SlackException($"Unexpected action type: {type}");
            }
        }
    }
}
