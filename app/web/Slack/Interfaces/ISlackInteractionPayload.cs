using Newtonsoft.Json;

namespace LangBot.Web.Slack
{
    [JsonConverter(typeof(SlackInteractionPayloadConverter))]
    public interface ISlackInteractionPayload : ISlackRequest
    {
        string CallbackId { get; }
    }
}
