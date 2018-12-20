namespace LangBot.Web.Slack
{
    // https://api.slack.com/methods/dialog.open
    public class SlackApiDialogOpenRequest
    {
        public SlackDialog Dialog { get; set; }
        public string TriggerId { get; set; }
    }
}
