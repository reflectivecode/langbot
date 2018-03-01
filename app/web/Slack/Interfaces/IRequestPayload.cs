using System;
using Newtonsoft.Json;

namespace LangBot.Web.Slack
{
    [JsonConverter(typeof(RequestPayloadConverter))]
    public interface IRequestPayload : ISlackRequest
    {
        string CallbackId { get; }
    }
}
