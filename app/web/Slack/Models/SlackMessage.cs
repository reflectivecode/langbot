using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class SlackMessage : IRequestResponse
    {
        public IList<SlackMessageAttachment> Attachments { get; set; }
        public bool? DeleteOriginal { get; set; }
        public bool? ReplaceOriginal { get; set; }
        public MessageResponseTypes? ResponseType { get; set; }
        public string Text { get; set; }

        public bool IsEmptyResponse() => false;
    }
}
