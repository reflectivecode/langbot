using System;
using System.Threading.Tasks;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web
{
    public class SwitchActionResponder : BaseActionResponder
    {
        protected override string ActionName => Constants.ActionNames.Switch;
        protected override MessageState AllowedMessageStates => MessageState.Preview;

        private readonly LangResponse _langResponse;
        private readonly ConfigService _configService;
        private readonly ImageUtility _imageUtility;

        public SwitchActionResponder(DatabaseRepo databaseRepo, LangResponse langResponse, ConfigService configService, ImageUtility imageUtility):base(databaseRepo)
        {
            _langResponse = langResponse;
            _configService = configService;
            _imageUtility = imageUtility;
        }

        protected override async Task<SlackMessage> Respond(SlackActionPayload payload, MemeMessage message)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (message == null) throw new ArgumentNullException(nameof(message));

            var template = await _configService.GetTemplate(payload.ActionValue, message.UserId);
            var imageUrl = await _imageUtility.GetImageUrl(message.Message, template);

            var updatedMessage = await DatabaseRepo.UpdatePreview(
                guid: message.Guid,
                templateId: template.Id,
                message: message.Message,
                imageUrl: imageUrl);

            if (updatedMessage == null || updatedMessage.PublishDate.HasValue || updatedMessage.DeleteDate.HasValue) return await _langResponse.RenderDelete();

            return await _langResponse.RenderPreview(updatedMessage);
        }
    }
}
