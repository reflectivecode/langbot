using System;
using System.Threading.Tasks;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web.Interactions
{
    public class LangUpVoteInteraction : BaseMemeInteraction
    {
        private readonly LangResponse _langResponse;
        private readonly DatabaseRepo _databaseRepo;
        private readonly DialogService _dialogService;

        public LangUpVoteInteraction(LangResponse langResponse, DatabaseRepo databaseRepo, DialogService dialogService)
        {
            _langResponse = langResponse;
            _databaseRepo = databaseRepo;
            _dialogService = dialogService;
        }

        protected override string ActionName => Constants.ActionNames.UpVote;

        protected async override Task<SlackMessage> Respond(SlackInteractionPayload payload, Guid guid)
        {
            var alreadyUpvoted = await _databaseRepo.HasReacted(guid, Constants.Reactions.UpVote, payload.User.Id);
            if (alreadyUpvoted)
            {
                await _dialogService.CreateDialog(payload.ActionTs, null);
                return null;
            }
            else
            {
                var message = await _databaseRepo.AddReaction(guid, Constants.Reactions.UpVote, payload.User.Id, payload.User.Name, null);
                return await _langResponse.RenderPublished(message);
            }
        }
    }
}
