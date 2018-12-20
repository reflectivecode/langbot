using System;
using Newtonsoft.Json.Linq;

namespace LangBot.Web.Slack
{
    public class SlackMessageActionConverter : SlackBaseReadConverter<ISlackMessageAction>
    {
        protected override Type GetObjectType(JObject jObject)
        {
            var type = jObject["type"].Value<string>();
            switch (type)
            {
                case "button": return typeof(SlackMessageButton);
                case "select": return typeof(SlackMessageSelect);
                default: throw new SlackException($"Unexpected action type: {type}");
            }
        }
    }
}
