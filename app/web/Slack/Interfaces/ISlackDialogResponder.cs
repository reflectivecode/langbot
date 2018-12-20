using System.Threading.Tasks;

namespace LangBot.Web.Slack
{
    public interface ISlackDialogResponder
    {
        Task<ISlackDialogResponse> Respond(SlackDialogPayload payload);
    }
}
