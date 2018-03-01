using System.Threading.Tasks;

namespace LangBot.Web.Slack
{
    public interface ICommand
    {
        Task<Message> Respond(CommandRequest command);
    }
}
