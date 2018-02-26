using System;
using System.Threading.Tasks;
using LangBot.Web.Models;
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

        public async Task<Message> Respond(CommandRequest command)
        {
            if (command.Command != Constants.Commands.Lang) return null;

            return await _langResponse.Preview(new PreviewModel
            {
                Text = command.Text,
                UserId = command.UserId,
            });
        }
    }
}
