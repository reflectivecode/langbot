using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class SlackDialogResponse : IRequestResponse
    {
        public IList<SlackDialogError> Errors { get; set; }

        public bool IsEmptyResponse() => Errors == null || Errors.Count == 0;
    }
}
