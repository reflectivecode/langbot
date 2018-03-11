using System.Threading.Tasks;

namespace LangBot.Web.Slack
{
    public interface ICommand
    {
        Task<SlackMessage> Respond(SlackCommandRequest command);
    }
}
