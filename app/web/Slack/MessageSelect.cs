using System;
using System.Collections.Generic;
using LangBot.Web.Enums;

namespace LangBot.Web.Slack
{
    public class MessageSelect : IMessageAction
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public MessageActionTypes Type { get; } = MessageActionTypes.Select;
        public string Value { get; set; }
        public IList<MessageOption> SelectedOptions { get; set; }
        public IList<MessageOption> Options { get; set; }
        public IList<MessageOptionGroup> OptionGroups { get; set; }
        public string GetValue() => SelectedOptions[0].Value;
    }
}
