using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class SlackMessageOptionGroup
    {
        public string Text { get; set; }
        public IList<SlackMessageOption> Options { get; set; }
    }
}
