using System;
using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class MessageAttachment
    {
        public IList<IMessageAction> Actions { get; set; }
        public string CallbackId { get; set; }
        public string Color { get; set; }
        public string Fallback { get; set; }
        public string ImageUrl { get; set; }
        public IList<string> MrkdwnIn { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
    }
}
