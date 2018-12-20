namespace LangBot.Web.Slack
{
    public class SlackDialogTextarea : ISlackDialogElement
    {
        public string Hint { get; set; }
        public string Label { get; set; }
        public int? MaxLength { get; set; }
        public int? MinLength { get; set; }
        public string Name { get; set; }
        public bool? Optional { get; set; }
        public string Placeholder { get; set; }
        public SlackDialogElementSubTypes? SubType { get; set; }
        public SlackDialogElementTypes Type => SlackDialogElementTypes.Textarea;
        public string Value { get; set; }
    }
}
