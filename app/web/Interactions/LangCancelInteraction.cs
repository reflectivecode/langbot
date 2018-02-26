using System;
using System.Threading.Tasks;
using LangBot.Web.Models;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web.Interactions
{
    public class LangCancelInteraction : IInteraction
    {
        private readonly LangResponse _langResponse;

        public LangCancelInteraction(LangResponse langResponse)
        {
            _langResponse = langResponse;
        }

        public async Task<Message> Respond(InteractionModel model)
        {
            if (model.CallbackId != Constants.CallbackIds.Meme) return null;
            if (model.ActionName != Constants.ActionNames.Cancel) return null;

            return await _langResponse.Cancel();
        }
    }
}
