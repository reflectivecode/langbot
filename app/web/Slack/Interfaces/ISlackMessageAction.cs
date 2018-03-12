using Newtonsoft.Json;

namespace LangBot.Web.Slack
{
    [JsonConverter(typeof(SlackMessageActionConverter))]
    public interface ISlackMessageAction
    {
        string Name { get; }
        string Text { get; }
        SlackMessageActionTypes Type { get; }
        string GetValue();
    }
}
