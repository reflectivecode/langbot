using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class DialogOpen
    {
        public string TriggerId { get; set; }
        public Dialog Dialog { get; set; }
    }
}
