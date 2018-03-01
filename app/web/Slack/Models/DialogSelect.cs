using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class DialogSelect : IDialogElement
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public bool? Optional { get; set; }
        public IList<DialogOption> Options { get; set; }
        public string Placeholder { get; set; }
        public DialogElementTypes Type => DialogElementTypes.Textarea;
        public string Value { get; set; }
    }
}
