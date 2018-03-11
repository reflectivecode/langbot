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

        protected async override Task<SlackMessage> Respond(InteractionModel model, Guid guid)
        {
            var alreadyUpvoted = await _databaseRepo.HasReacted(guid, Constants.Reactions.UpVote, model.Payload.User.Id);
            if (alreadyUpvoted)
            {
                await _dialogService.CreateDialog(model.Payload.ActionTs, null);
                return null;
            }
            else
            {
                var message = await _databaseRepo.AddReaction(guid, Constants.Reactions.UpVote, model.Payload.User.Id, model.Payload.User.Name, null);
                return await _langResponse.RenderPublished(message);
            }
        }
    }
}
