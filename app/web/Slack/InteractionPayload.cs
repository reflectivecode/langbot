using System;
using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class InteractionPayload : SlackRequest
    {
        public IList<IMessageAction> Actions { get; set; }
        public string CallbackId { get; set; }
    }
}
