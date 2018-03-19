using System;
using System.Linq;
using System.Threading.Tasks;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web
{
    public class EditActionResponder : BaseActionResponder
    {
        protected override string ActionName => Constants.ActionNames.Edit;
        protected override MessageState AllowedMessageStates => MessageState.Preview;

        private readonly SlackClient _slackClient;
        private readonly ConfigService _configService;
        private readonly TextSplitter _textSplitter;

        public EditActionResponder(DatabaseRepo databaseRepo, SlackClient slackClient, ConfigService configService, TextSplitter textSplitter) : base(databaseRepo)
        {
            _slackClient = slackClient;
            _configService = configService;
            _textSplitter = textSplitter;
        }

        protected override async Task<ISlackActionResponse> Respond(SlackActionPayload payload, MemeMessage message)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (message == null) throw new ArgumentNullException(nameof(message));

            var config = await _configService.GetConfig();
            var template = await _configService.GetTemplate(message.TemplateId, message.UserId);
            var boxes = template.Boxes ?? config.TemplateDefaults.Boxes;
            var lines = _textSplitter.SplitText(message.Message, boxes.Count);

            var elements = lines.SelectWithIndex((line, i) => new SlackDialogText
            {
                Label = $"Line {i}",
                Name = $"line{i}",
                Optional = true,
                Value = line,
            }).ToList<ISlackDialogElement>();

            elements.Add(new SlackDialogSelect
            {
                Label = "Post anonymously",
                Name = "isAnonymous",
                Optional = false,
                Value = message.IsAnonymous.ToString(),
                Options = new[]
                {
                    new SlackDialogOption
                    {
                        Label = "Yes",
                        Value = Boolean.TrueString,
                    },
                    new SlackDialogOption
                    {
                        Label = "No",
                        Value = Boolean.FalseString,
                    },
                }
            });

            await _slackClient.DialogOpen(new SlackApiDialogOpenRequest
            {
                TriggerId = payload.TriggerId,
                Dialog = new SlackDialog
                {
                    CallbackId = $"{Constants.CallbackIds.Edit}:{message.Guid}",
                    Title = "Edit",
                    SubmitLabel = "Preview",
                    Elements = elements,
                }
            });

            return new SlackEmptyResponse();
        }
    }
}
