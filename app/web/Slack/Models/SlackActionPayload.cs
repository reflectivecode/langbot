using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    // https://api.slack.com/docs/interactive-message-field-guide#action_payload
    public class SlackActionPayload : ISlackInteractionPayload
    {
        public IList<ISlackMessageAction> Actions { get; set; }
        public string CallbackId { get; set; }
        public SlackChannel Channel { get; set; }
        public string ResponseUrl { get; set; }
        public SlackTeam Team { get; set; }
        public string Token { get; set; }
        public string TriggerId { get; set; }
        public SlackUser User { get; set; }

        // Convenience Properties
        public string ActionName => Actions[0].Name;
        public string ActionValue => Actions[0].GetValue();
    }
}
