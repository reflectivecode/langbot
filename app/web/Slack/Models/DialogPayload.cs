using System;
using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class DialogPayload : IRequestPayload
    {
        public IDictionary<string, string> Submission { get; set; }
        public string CallbackId { get; set; }
        public string Token { get; set; }
    }
}
