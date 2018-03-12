using System.Threading.Tasks;

namespace LangBot.Web.Slack
{
    public interface ISlackDialogResponder
    {
        Task<SlackDialogResponse> Respond(SlackDialogPayload payload);
    }
}
