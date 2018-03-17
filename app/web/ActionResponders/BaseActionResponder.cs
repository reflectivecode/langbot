using System;
using System.Threading.Tasks;
using LangBot.Web.Slack;
using EnumsNET;

namespace LangBot.Web
{
    public abstract class BaseActionResponder : ISlackActionResponder
    {
        protected abstract string ActionName { get; }
        protected abstract MessageState AllowedMessageStates { get; }

        protected DatabaseRepo DatabaseRepo { get; }

        public BaseActionResponder(DatabaseRepo databaseRepo)
        {
            DatabaseRepo = databaseRepo;
        }

        protected abstract Task<ISlackActionResponse> Respond(SlackActionPayload payload, MemeMessage message);

        public async Task<ISlackActionResponse> Respond(SlackActionPayload payload)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));

            if (payload.CallbackId != Constants.CallbackIds.Meme) return null;
            if (String.IsNullOrEmpty(payload.ActionName)) return null;
            if (!payload.ActionName.StartsWith(ActionName + ":")) return null;
            var guid = Guid.Parse(ActionName.Substring(ActionName.Length + 1));

            var message = await DatabaseRepo.SelectMessage(guid);
            if (message == null) throw new SlackException("Message not found in database");
            if (!AllowedMessageStates.HasAnyFlags(message.MessageState)) throw new SlackException($"Message is not in a valid state for this action. Message state: {message.MessageState}, valid state: {AllowedMessageStates}");
            if (message.MessageState == MessageState.Preview && payload.User.Id != message.UserId) throw new SlackException("Invalid access. UserId does not match.");
            if (message.TeamId != payload.Team.Id) throw new SlackException("Invalid access. TeamId does not match.");
            if (message.ChannelId != payload.Channel.Id) throw new SlackException("Invalid access. ChannelId does not match.");

            return await Respond(payload, message);
        }
    }
}
