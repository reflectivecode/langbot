using System;
using Newtonsoft.Json.Linq;

namespace LangBot.Web.Slack
{
    public class RequestPayloadConverter : BaseReadConverter<IRequestPayload>
    {
        protected override Type GetObjectType(JObject jObject)
        {
            var type = jObject["type"].Value<string>();
            switch (type)
            {
                case null: return typeof(InteractionPayload);
                case "dialog_submission": return typeof(DialogPayload);
                default: throw new SlackException($"Unexpected request type: {type}");
            }
        }
    }
}
