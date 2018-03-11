using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class SlackInteractionPayload : IRequestPayload
    {
        public IList<IMessageAction> Actions { get; set; }
        public string ActionTs { get; set; }
        public string CallbackId { get; set; }
        public string Token { get; set; }
        public UserHash User { get; set; }
    }
}
