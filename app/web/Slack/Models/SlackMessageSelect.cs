using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class SlackMessageSelect : ISlackMessageAction
    {
        public string Name { get; set; }
        public IList<SlackMessageOptionGroup> OptionGroups { get; set; }
        public IList<SlackMessageOption> Options { get; set; }
        public IList<SlackMessageOption> SelectedOptions { get; set; }
        public string Text { get; set; }
        public SlackMessageActionTypes Type => SlackMessageActionTypes.Select;
        public string Value { get; set; }

        public string GetValue() => SelectedOptions[0].Value;
    }
}
