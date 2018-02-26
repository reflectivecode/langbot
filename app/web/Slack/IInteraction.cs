using System.Threading.Tasks;

namespace LangBot.Web.Slack
{
    public interface IInteraction
    {
        Task<Message> Respond(InteractionModel model);
    }
}
