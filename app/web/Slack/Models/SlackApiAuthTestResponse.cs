namespace LangBot.Web.Slack
{
    // https://api.slack.com/methods/auth.test
    public class SlackApiAuthTestResponse : SlackApiBaseResponse
    {
        public string Url { get; set; }
        public string Team { get; set; }
        public string User { get; set; }
        public string TeamId { get; set; }
        public string UserId { get; set; }
    }
}
