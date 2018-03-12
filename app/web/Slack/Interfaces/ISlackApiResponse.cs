namespace LangBot.Web.Slack
{
    public interface ISlackApiResponse
    {
        bool Ok { get; }
        string Warning { get; }
        string Error { get; }
    }
}
