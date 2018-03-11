using System.Threading.Tasks;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web.Commands
{
    public class LangCommand : ICommand
    {
        private readonly LangResponse _langResponse;

        public LangCommand(LangResponse langResponse)
        {
            _langResponse = langResponse;
        }

        public async Task<SlackMessage> Respond(SlackCommandRequest command)
        {
            if (command.Command != Constants.Commands.Lang) return null;

            var message = await _langResponse.CreateMessage(
                teamId: command.TeamId,
                teamDomain: command.TeamDomain,
                channelId: command.ChannelId,
                channelName: command.ChannelName,
                userId: command.UserId,
                userName: command.UserName,
                message: command.Text);

            return await _langResponse.RenderPreview(message);
        }
    }
}
