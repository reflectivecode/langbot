using Newtonsoft.Json;

namespace LangBot.Web.Slack
{
    [JsonConverter(typeof(SlackRequestPayloadConverter))]
    public interface ISlackInteractionPayload : ISlackRequest
    {
        string CallbackId { get; }
    }
}
