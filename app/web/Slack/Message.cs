using System;
using System.Collections.Generic;
using LangBot.Web.Enums;

namespace LangBot.Web.Slack
{
    public class Message
    {
        public IList<MessageAttachment> Attachments { get; set; }
        public bool? DeleteOriginal { get; set; }
        public bool? ReplaceOriginal { get; set; }
        public MessageResponseTypes? ResponseType { get; set; }
        public string Text { get; set; }
    }
}
