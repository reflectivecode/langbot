using System.Threading.Tasks;

namespace LangBot.Web.Slack
{
    public interface ISlackCommandResponder
    {
        Task<SlackMessage> Respond(SlackCommandRequest command);
    }
}
