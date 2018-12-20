using System;
using System.Threading.Tasks;
using LangBot.Web.Slack;
using LangBot.Web.Services;

namespace LangBot.Web
{
    public class PostSubmitActionResponder : ISlackActionResponder
    {
        private readonly Serializer _serializer;
        private readonly ConfigService _configService;

        public PostSubmitActionResponder(Serializer serializer, ConfigService configService)
        {
            _serializer = serializer;
            _configService = configService;
        }

        public async Task<ISlackActionResponse> Respond(SlackActionPayload payload)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));

            if (payload.CallbackId != Constants.CallbackIds.Post) return null;
            if (payload.ActionName != Constants.ActionNames.Submit) return null;

            if (!await _configService.IsPrivilegedUser(payload.User.Id)) throw new SlackException("User does not have permission to do this action.");

            var message = _serializer.Base64UrlToObject<string>(payload.ActionValue);

            return new SlackMessage
            {
                ResponseType = SlackMessageResponseTypes.InChannel,
                DeleteOriginal = true,
                Text = message
            };
        }
    }
}
