using System.Collections.Generic;

namespace LangBot.Web.Slack
{
    public class DialogResponse : IRequestResponse
    {
        public IList<DialogError> Errors { get; set; }

        public bool IsEmptyResponse() => Errors == null || Errors.Count == 0;
    }
}
