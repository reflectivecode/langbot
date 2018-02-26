using System;
using System.Collections.Generic;
using LangBot.Web.Enums;

namespace LangBot.Web.Slack
{
    public class MessageButton : IMessageAction
    {
        public string Name { get; set; }
        public MessageButtonStyles Style { get; set; } = MessageButtonStyles.Default;
        public string Text { get; set; }
        public MessageActionTypes Type { get; } = MessageActionTypes.Button;
        public string Value { get; set; }

        public string GetValue() => Value;
    }
}
