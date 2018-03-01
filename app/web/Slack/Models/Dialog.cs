using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class Dialog
    {
        public string CallbackId { get; set; }
        public IList<IDialogElement> Elements { get; set; } = new List<IDialogElement>();
        public string SubmitLabel { get; set; }
        public string Title { get; set; }
    }
}
