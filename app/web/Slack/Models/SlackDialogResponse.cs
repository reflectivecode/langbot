using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class SlackDialogResponse : ISlackInteractionResponse
    {
        public IList<SlackDialogError> Errors { get; set; }

        public bool IsEmptyResponse() => Errors == null || Errors.Count == 0;
    }
}
