using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class SlackDialogPayload : ISlackInteractionPayload
    {
        public string CallbackId { get; set; }
        public SlackChannel Channel { get; set; }
        public IDictionary<string, string> Submission { get; set; }
        public SlackTeam Team { get; set; }
        public string Token { get; set; }
        public SlackUser User { get; set; }
    }
}
