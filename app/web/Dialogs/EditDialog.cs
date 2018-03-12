using System;
using System.Text;
using System.Threading.Tasks;
using LangBot.Web.Models;
using LangBot.Web.Services;
using LangBot.Web.Slack;

namespace LangBot.Web.Interactions
{
    public class EditDialog : ISlackDialogResponder
    {
        private readonly LangResponse _langResponse;
        private readonly Serializer _serializer;
        private readonly SlackResponseClient _responseClient;

        public EditDialog(LangResponse langResponse, Serializer serializer, SlackResponseClient responseClient)
        {
            _langResponse = langResponse;
            _serializer = serializer;
            _responseClient = responseClient;
        }

        public async Task<SlackDialogResponse> Respond(SlackDialogPayload payload)
        {
            if (payload.CallbackId != Constants.CallbackIds.Edit) return null;

            var anonymous = payload.Submission[Constants.DialogElements.Anonymous];
            var model = _serializer.Base64UrlToObject<EditDialogModel>(anonymous);

            var text = new StringBuilder();
            for (int i = 0; true; i++)
            {
                if (!payload.Submission.TryGetValue($"text{0}", out var value)) break;
                if (i > 0) text.Append("; ");
                text.Append(value);
            }

            var message = await _langResponse.Preview(new PreviewModel
            {
                Text = text.ToString(),
                TemplateId = model.TemplateId,
                UserId = model.UserId,
                Anonymous = model.Anonymous,
            });

            await _responseClient.Post<object>(model.ResponseUrl, message);

            return new SlackDialogResponse();
        }
    }
}
