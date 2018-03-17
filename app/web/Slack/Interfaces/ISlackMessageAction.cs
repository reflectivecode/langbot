using Newtonsoft.Json;

namespace LangBot.Web.Slack
{
    public interface ISlackMessageAction
    {
        string Name { get; }
        string Text { get; }
        SlackMessageActionTypes Type { get; }
        string GetValue();
    }
}
