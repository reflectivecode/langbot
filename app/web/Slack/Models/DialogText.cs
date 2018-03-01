using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class DialogText : IDialogElement
    {
        public string Hint { get; set; }
        public string Label { get; set; }
        public int? MaxLength { get; set; }
        public int? MinLength { get; set; }
        public string Name { get; set; }
        public bool? Optional { get; set; }
        public string Placeholder { get; set; }
        public DialogElementSunTypes? SubType { get; set; }
        public DialogElementTypes Type => DialogElementTypes.Text;
        public string Value { get; set; }
    }
}
