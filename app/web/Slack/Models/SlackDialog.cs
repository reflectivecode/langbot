using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class SlackDialog
    {
        public string CallbackId { get; set; }
        public IList<ISlackDialogElement> Elements { get; set; } = new List<ISlackDialogElement>();
        public string SubmitLabel { get; set; }
        public string Title { get; set; }
    }
}
