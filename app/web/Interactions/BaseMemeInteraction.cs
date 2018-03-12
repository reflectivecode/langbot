using System;
using System.Threading.Tasks;
using LangBot.Web.Slack;

namespace LangBot.Web.Interactions
{
    public abstract class BaseMemeInteraction : ISlackInteractionResponder
    {
        protected abstract string ActionName { get; }

        protected abstract Task<SlackMessage> Respond(InteractionModel model, Guid guid);

        public async Task<SlackMessage> Respond(InteractionModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            if (model.CallbackId != Constants.CallbackIds.Meme) return null;
            if (String.IsNullOrEmpty(model.ActionName)) return null;
            if (!model.ActionName.StartsWith(ActionName + ":")) return null;
            var guid = Guid.Parse(ActionName.Substring(ActionName.Length + 1));
            return await Respond(model, guid);
        }
    }
}
