using System;
using System.Threading.Tasks;
using LangBot.Web.Models;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web.Interactions
{
    public class LangSubmitInteraction : IInteraction
    {
        private readonly LangResponse _langResponse;
        private readonly Serializer _serializer;

        public LangSubmitInteraction(LangResponse langResponse, Serializer serializer)
        {
            _langResponse = langResponse;
            _serializer = serializer;
        }

        public async Task<Message> Respond(InteractionModel model)
        {
            if (model.CallbackId != Constants.CallbackIds.Meme) return null;
            if (model.ActionName != Constants.ActionNames.Submit) return null;

            var value = _serializer.Base64UrlToObject<SubmitModel>(model.ActionValue);
            return await _langResponse.Submit(value);
        }
    }
}
