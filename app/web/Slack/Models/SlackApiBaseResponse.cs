namespace LangBot.Web.Slack
{
    public abstract class SlackApiBaseResponse : ISlackApiResponse
    {
        public string Error { get; set; }
        public bool Ok { get; set; }
        public string Warning { get; set; }
    }
}
