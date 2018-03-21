using System;
using System.Threading.Tasks;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web
{
    public class RawActionResponder : BaseActionResponder
    {
        protected override string ActionName => Constants.ActionNames.Raw;
        protected override MessageState AllowedMessageStates => MessageState.Preview;

        private readonly ConfigService _configService;

        public RawActionResponder(DatabaseRepo databaseRepo, ConfigService configService) : base(databaseRepo)
        {
            _configService = configService;
        }

        protected override async Task<ISlackActionResponse> Respond(SlackActionPayload payload, MemeMessage message)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (message == null) throw new ArgumentNullException(nameof(message));

            var isPrivilegedUser = await _configService.IsPrivilegedUser(message.UserId);
            if (!isPrivilegedUser) throw new SlackException("Not permissioned to post raw messages.");

            await DatabaseRepo.DeletePreview(message.Id);

            return new SlackMessage
            {
                DeleteOriginal = true,
                ResponseType = SlackMessageResponseTypes.InChannel,
                Text = message.Message,
            };
        }
    }
}
