using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class SlackMessageSelect : IMessageAction
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public MessageActionTypes Type { get; } = MessageActionTypes.Select;
        public string Value { get; set; }
        public IList<SlackMessageOption> SelectedOptions { get; set; }
        public IList<SlackMessageOption> Options { get; set; }
        public IList<SlackMessageOptionGroup> OptionGroups { get; set; }
        public string GetValue() => SelectedOptions[0].Value;
    }
}
