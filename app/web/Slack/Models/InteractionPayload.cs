using System;
using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class InteractionPayload : IRequestPayload
    {
        public IList<IMessageAction> Actions { get; set; }
        public string CallbackId { get; set; }
        public string Token { get; set; }
    }
}
