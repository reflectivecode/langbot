using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class SlackInteractionPayload : ISlackInteractionPayload
    {
        public IList<ISlackMessageAction> Actions { get; set; }
        public string ActionTs { get; set; }
        public string CallbackId { get; set; }
        public string Token { get; set; }
        public SlackUser User { get; set; }

        // Convenience Properties
        public string ActionName => Actions[0].Name;        public string ActionValue => Actions[0].GetValue();    }
}
