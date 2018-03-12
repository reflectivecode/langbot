using System;
using System.Threading.Tasks;
using LangBot.Web.Models;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web.Interactions
{
    public class LangEditInteraction : ISlackInteractionResponder
    {
        private readonly LangResponse _langResponse;
        private readonly Serializer _serializer;

        public LangEditInteraction(LangResponse langResponse, Serializer serializer)
        {
            _langResponse = langResponse;
            _serializer = serializer;
        }

        public async Task<SlackMessage> Respond(InteractionModel model)
        {
            if (model.CallbackId != Constants.CallbackIds.Meme) return null;
            if (model.ActionName != Constants.ActionNames.Switch) return null;

            var value = _serializer.Base64UrlToObject<PreviewModel>(model.ActionValue);
            return await _langResponse.Preview(value);
        }
    }
}
