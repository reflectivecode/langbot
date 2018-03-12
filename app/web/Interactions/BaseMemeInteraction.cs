using System;
using System.Threading.Tasks;
using LangBot.Web.Slack;

namespace LangBot.Web.Interactions
{
    public abstract class BaseMemeInteraction : ISlackInteractionResponder
    {
        protected abstract string ActionName { get; }

        protected abstract Task<SlackMessage> Respond(SlackInteractionPayload payload, Guid guid);

        public async Task<SlackMessage> Respond(SlackInteractionPayload payload)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));

            if (payload.CallbackId != Constants.CallbackIds.Meme) return null;
            if (String.IsNullOrEmpty(payload.ActionName)) return null;
            if (!payload.ActionName.StartsWith(ActionName + ":")) return null;
            var guid = Guid.Parse(ActionName.Substring(ActionName.Length + 1));
            return await Respond(payload, guid);
        }
    }
}
