using System;
using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class Message : IRequestResponse
    {
        public IList<MessageAttachment> Attachments { get; set; }
        public bool? DeleteOriginal { get; set; }
        public bool? ReplaceOriginal { get; set; }
        public MessageResponseTypes? ResponseType { get; set; }
        public string Text { get; set; }

        public bool IsEmptyResponse() => false;
    }
}
