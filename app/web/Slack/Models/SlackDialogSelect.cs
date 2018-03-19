using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class SlackDialogSelect : ISlackDialogElement
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public bool? Optional { get; set; }
        public IList<SlackDialogOption> Options { get; set; }
        public string Placeholder { get; set; }
        public SlackDialogElementTypes Type => SlackDialogElementTypes.Select;
        public string Value { get; set; }
    }
}
