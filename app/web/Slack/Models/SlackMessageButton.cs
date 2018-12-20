namespace LangBot.Web.Slack
{
    public class SlackMessageButton : ISlackMessageAction
    {
        public string Name { get; set; }
        public SlackMessageButtonStyles Style { get; set; } = SlackMessageButtonStyles.Default;
        public string Text { get; set; }
        public SlackMessageActionTypes Type => SlackMessageActionTypes.Button;
        public string Value { get; set; }

        public string GetValue() => Value;
    }
}
