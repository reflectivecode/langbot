using System;
using System.Text;
using System.Threading.Tasks;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web.Interactions
{
    public class EditDialogResponder : BaseDialogResponder
    {
        protected override string CallbackName => Constants.CallbackIds.Edit;
        protected override MessageState AllowedMessageStates => MessageState.Preview;

        private readonly LangResponse _langResponse;
        private readonly SlackClient _slackClient;
        private readonly ImageUtility _imageUtility;
        private readonly ConfigService _configService;

        public EditDialogResponder(DatabaseRepo databaseRepo, LangResponse langResponse, SlackClient slackClient, ImageUtility imageUtility, ConfigService configService) : base(databaseRepo)
        {
            _langResponse = langResponse;
            _slackClient = slackClient;
            _imageUtility = imageUtility;
            _configService = configService;
        }

        protected override async Task<ISlackDialogResponse> Respond(SlackDialogPayload payload, MemeMessage message, Response response)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (response == null) throw new ArgumentNullException(nameof(response));

            var text = new StringBuilder();
            for (int i = 0; true; i++)
            {
                if (!payload.Submission.TryGetValue($"line{i}", out var value)) break;
                if (i > 0) text.Append("; ");
                text.Append(value);
            }

            var isAnonymous = Boolean.Parse(payload.Submission["isAnonymous"]);
            var template = await _configService.GetTemplate(message.TemplateId, message.UserId);
            var imageUrl = await _imageUtility.GetImageUrl(text.ToString(), template);

            var updatedMessage = await DatabaseRepo.UpdatePreview(
                id: message.Id,
                templateId: message.TemplateId,
                message: text.ToString(),
                imageUrl: imageUrl,
                isAnonymous: isAnonymous);

            var slackMessage = await _langResponse.RenderPreview(updatedMessage);
            slackMessage.ReplaceOriginal = true;

            await _slackClient.SendMessageResponse(response.ResponseUrl, slackMessage);
            await DatabaseRepo.DeleteResponse(response.Id);

            return new SlackEmptyResponse();
        }
    }
}
