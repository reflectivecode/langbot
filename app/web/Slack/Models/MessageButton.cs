using System;
using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class MessageButton : IMessageAction
    {
        public string Name { get; set; }
        public MessageButtonStyles Style { get; set; } = MessageButtonStyles.Default;
        public string Text { get; set; }
        public MessageActionTypes Type => MessageActionTypes.Button;
        public string Value { get; set; }

        public string GetValue() => Value;
    }
}
