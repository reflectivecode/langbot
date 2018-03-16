using System.Threading.Tasks;

namespace LangBot.Web.Slack
{
    public interface ISlackActionResponder
    {
        Task<SlackMessage> Respond(SlackActionPayload model);
    }
}
