using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class MessageOptionGroup
    {
        public string Text { get; set; }
        public IList<MessageOption> Options { get; set; }
    }
}
