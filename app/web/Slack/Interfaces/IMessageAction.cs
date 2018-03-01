using System;
using Newtonsoft.Json;

namespace LangBot.Web.Slack
{
    [JsonConverter(typeof(MessageActionConverter))]
    public interface IMessageAction
    {
        string Name { get; }
        string Text { get; }
        MessageActionTypes Type { get; }
        string GetValue();
    }
}
