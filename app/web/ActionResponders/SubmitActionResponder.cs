using System;
using System.Threading.Tasks;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web
{
    public class SubmitActionResponder : BaseActionResponder
    {
        protected override string ActionName => Constants.ActionNames.Submit;
        protected override MessageState AllowedMessageStates => MessageState.Preview;

        private readonly LangResponse _langResponse;

        public SubmitActionResponder(DatabaseRepo databaseRepo, LangResponse langResponse) : base(databaseRepo)
        {
            _langResponse = langResponse;
        }

        protected async override Task<SlackMessage> Respond(SlackActionPayload payload, MemeMessage message)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (message == null) throw new ArgumentNullException(nameof(message));

            var updatedMessage = await DatabaseRepo.PublishMessage(message.Guid);
            if (updatedMessage == null || updatedMessage.DeleteDate.HasValue) return await _langResponse.RenderDelete();
            return await _langResponse.RenderPublished(message);
        }
    }
}
