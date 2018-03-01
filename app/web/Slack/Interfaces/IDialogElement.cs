namespace LangBot.Web.Slack
{
    public interface IDialogElement
    {
        string Label { get; }
        string Name { get; }
        bool? Optional { get; }
        string Placeholder { get; }
        DialogElementTypes Type { get; }
        string Value { get; }
    }
}
