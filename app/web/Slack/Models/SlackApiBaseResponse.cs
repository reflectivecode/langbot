namespace LangBot.Web.Slack
{
    public class SlackApiBaseResponse : ISlackApiResponse
    {
        public string Error { get; set; }
        public bool Ok { get; set; }
        public string Warning { get; set; }
    }
}
