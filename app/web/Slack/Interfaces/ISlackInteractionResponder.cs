using System.Threading.Tasks;

namespace LangBot.Web.Slack
{
    public interface ISlackInteractionResponder
    {
        Task<SlackMessage> Respond(SlackInteractionPayload model);
    }
}
