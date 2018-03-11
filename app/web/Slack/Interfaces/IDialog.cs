using System.Threading.Tasks;

namespace LangBot.Web.Slack
{
    public interface IDialog
    {
        Task<SlackDialogResponse> Respond(SlackDialogPayload payload);
    }
}
