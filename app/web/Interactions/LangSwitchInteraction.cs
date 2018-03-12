using System;
using System.Threading.Tasks;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web.Interactions
{
    public class LangSwitchInteraction : BaseMemeInteraction
    {
        private readonly LangResponse _langResponse;
        private readonly DatabaseRepo _databaseRepo;
        private readonly ConfigService _configService;
        private readonly ImageUtility _imageUtility;

        public LangSwitchInteraction(LangResponse langResponse, DatabaseRepo databaseRepo, ConfigService configService, ImageUtility imageUtility)
        {
            _langResponse = langResponse;
            _databaseRepo = databaseRepo;
            _configService = configService;
            _imageUtility = imageUtility;
        }

        protected override string ActionName => Constants.ActionNames.Switch;

        protected async override Task<SlackMessage> Respond(SlackInteractionPayload payload, Guid guid)
        {
            var originalMessage = await _databaseRepo.SelectMessage(guid);
            if (originalMessage == null || originalMessage.PublishDate.HasValue || originalMessage.DeleteDate.HasValue) return await _langResponse.RenderDelete();

            var template = await _configService.GetTemplate(payload.ActionValue, originalMessage.UserId);
            var imageUrl = await _imageUtility.GetImageUrl(originalMessage.Message, template);

            var updatedMessage = await _databaseRepo.UpdatePreview(
                guid: guid,
                templateId: template.Id,
                message: originalMessage.Message,
                imageUrl: imageUrl);

            if (updatedMessage == null || updatedMessage.PublishDate.HasValue || updatedMessage.DeleteDate.HasValue) return await _langResponse.RenderDelete();

            return await _langResponse.RenderPreview(updatedMessage);
        }
    }
}
