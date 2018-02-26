using System;

namespace LangBot.Web.Slack
{
    public class SlackException : Exception
    {
        public SlackException(string message) : base(message) { }
    }
}
