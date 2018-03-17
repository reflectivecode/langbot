using System.Threading.Tasks;

namespace LangBot.Web.Slack
{
    public interface ISlackActionResponder
    {
        Task<ISlackActionResponse> Respond(SlackActionPayload model);
    }
}
