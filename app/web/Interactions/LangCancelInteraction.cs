using System;
using System.Threading.Tasks;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web.Interactions
{
    public class LangCancelInteraction : BaseMemeInteraction
    {
        private readonly LangResponse _langResponse;
        private readonly DatabaseRepo _databaseRepo;

        public LangCancelInteraction(LangResponse langResponse, DatabaseRepo databaseRepo)
        {
            _langResponse = langResponse;
            _databaseRepo = databaseRepo;
        }

        protected override string ActionName => Constants.ActionNames.Cancel;

        protected async override Task<SlackMessage> Respond(SlackInteractionPayload payload, Guid guid)
        {
            await _databaseRepo.DeletePreview(guid);
            return await _langResponse.RenderDelete();
        }
    }
}
