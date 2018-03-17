using System;
using System.Threading.Tasks;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web
{
    public class CancelActionResponder : BaseActionResponder
    {
        protected override string ActionName => Constants.ActionNames.Cancel;
        protected override MessageState AllowedMessageStates => MessageState.Preview;

        private readonly LangResponse _langResponse;

        public CancelActionResponder(DatabaseRepo databaseRepo, LangResponse langResponse) : base(databaseRepo)
        {
            _langResponse = langResponse;
        }

        protected override async Task<ISlackActionResponse> Respond(SlackActionPayload payload, MemeMessage message)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (message == null) throw new ArgumentNullException(nameof(message));

            await DatabaseRepo.DeletePreview(message.Id);
            return await _langResponse.RenderDelete();
        }
    }
}
