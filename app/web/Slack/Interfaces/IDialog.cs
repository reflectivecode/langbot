using System.Threading.Tasks;

namespace LangBot.Web.Slack
{
    public interface IDialog
    {
        Task<DialogResponse> Respond(DialogPayload payload);
    }
}
