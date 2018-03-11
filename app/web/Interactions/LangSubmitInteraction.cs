using System;
using System.Threading.Tasks;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web.Interactions
{
    public class LangSubmitInteraction : BaseMemeInteraction
    {
        private readonly LangResponse _langResponse;
        private readonly DatabaseRepo _databaseRepo;

        public LangSubmitInteraction(LangResponse langResponse, DatabaseRepo databaseRepo)
        {
            _langResponse = langResponse;
            _databaseRepo = databaseRepo;
        }

        protected override string ActionName => Constants.ActionNames.Submit;

        protected async override Task<Message> Respond(InteractionModel model, Guid guid)
        {
            var message = await _databaseRepo.PublishMessage(guid);
            if (message == null || message.DeleteDate.HasValue) return await _langResponse.RenderDelete();
            return await _langResponse.RenderPublished(message);
        }
    }
}
