using System.Threading.Tasks;

namespace LangBot.Web.Slack
{
    public interface IInteraction
    {
        Task<SlackMessage> Respond(InteractionModel model);
    }
}
