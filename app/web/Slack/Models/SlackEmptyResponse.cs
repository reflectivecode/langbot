namespace LangBot.Web.Slack
{
    public class SlackEmptyResponse : ISlackActionResponse, ISlackDialogResponse
    {
        public bool IsEmptyResponse() => true;
    }
}
