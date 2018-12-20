namespace LangBot.Web.Slack
{
    // https://api.slack.com/methods/api.test
    public class SlackApiTestResponse : SlackApiBaseResponse
    {
        public ArgsModel Args { get; set; }

        public class ArgsModel
        {
            public string Error { get; set; }
            public string Foo { get; set; }
        }
    }
}
