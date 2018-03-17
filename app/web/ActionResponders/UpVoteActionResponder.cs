using System;
using System.Threading.Tasks;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web
{
    public class UpVoteActionResponder : BaseActionResponder
    {
        protected override string ActionName => Constants.ActionNames.UpVote;
        protected override MessageState AllowedMessageStates => MessageState.Published;

        private readonly LangResponse _langResponse;

        public UpVoteActionResponder(DatabaseRepo databaseRepo, LangResponse langResponse): base(databaseRepo)
        {
            _langResponse = langResponse;
        }

        protected async override Task<ISlackActionResponse> Respond(SlackActionPayload payload, MemeMessage message)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (message == null) throw new ArgumentNullException(nameof(message));

            var alreadyUpvoted = await DatabaseRepo.HasReacted(message.Id, Constants.Reactions.UpVote, payload.User.Id);
            if (alreadyUpvoted)
                return await RemoveVote(payload, message);
            else
                return await AddVote(payload, message);
        }

        private async Task<SlackMessage> RemoveVote(SlackActionPayload payload, MemeMessage message)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (message == null) throw new ArgumentNullException(nameof(message));

            var updatedMessage = await DatabaseRepo.RemoveReaction(message.Id, Constants.Reactions.UpVote, payload.User.Id);
            return await _langResponse.RenderPublished(updatedMessage);
        }

        private async Task<SlackMessage> AddVote(SlackActionPayload payload, MemeMessage message)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (message == null) throw new ArgumentNullException(nameof(message));

            var updatedMessage = await DatabaseRepo.AddReaction(message.Id, Constants.Reactions.UpVote, payload.User.Id, payload.User.Name, null);
            return await _langResponse.RenderPublished(updatedMessage);
        }
    }
}
