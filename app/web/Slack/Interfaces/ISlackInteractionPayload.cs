using Newtonsoft.Json;

namespace LangBot.Web.Slack
{
    public interface ISlackInteractionPayload : ISlackRequest
    {
        string CallbackId { get; }
    }
}
