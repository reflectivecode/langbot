namespace LangBot.Web.Slack
{
    public interface ISlackDialogElement
    {
        string Label { get; }
        string Name { get; }
        bool? Optional { get; }
        string Placeholder { get; }
        SlackDialogElementTypes Type { get; }
        string Value { get; }
    }
}
