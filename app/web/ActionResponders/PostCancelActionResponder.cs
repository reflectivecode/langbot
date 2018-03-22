using System;
using System.Threading.Tasks;
using LangBot.Web.Slack;

namespace LangBot.Web
{
    public class PostCancelActionResponder : ISlackActionResponder
    {
        public Task<ISlackActionResponse> Respond(SlackActionPayload payload)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));

            if (payload.CallbackId != Constants.CallbackIds.Post) return TaskFromResult(null);
            if (payload.ActionName != Constants.ActionNames.Cancel) return TaskFromResult(null);

            return TaskFromResult(new SlackMessage
            {
                DeleteOriginal = true,
            });
        }

        private Task<ISlackActionResponse> TaskFromResult(ISlackActionResponse response)
        {
            return Task.FromResult(response);
        }
    }
}
