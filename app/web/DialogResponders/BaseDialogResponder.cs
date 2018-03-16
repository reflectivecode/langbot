using System;
using System.Threading.Tasks;
using LangBot.Web.Slack;
using EnumsNET;

namespace LangBot.Web
{
    public abstract class BaseDialogResponder : ISlackDialogResponder
    {
        protected abstract string CallbackName { get; }
        protected abstract MessageState AllowedMessageStates { get; }

        protected DatabaseRepo DatabaseRepo { get; }

        public BaseDialogResponder(DatabaseRepo databaseRepo)
        {
            DatabaseRepo = databaseRepo;
        }

        protected abstract Task<SlackDialogResponse> Respond(SlackDialogPayload payload, MemeMessage message, Response response);

        public async Task<SlackDialogResponse> Respond(SlackDialogPayload payload)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));

            if (String.IsNullOrEmpty(payload.CallbackId)) return null;
            if (!payload.CallbackId.StartsWith(CallbackName + ":")) return null;

            var responseGuid = Guid.Parse(CallbackName.Substring(CallbackName.Length + 1));
            var response = await DatabaseRepo.SelectResponse(responseGuid);
            if (response == null) throw new SlackException("Response id not found in database");
            if (response.TeamId != payload.Team.Id) throw new SlackException("Invalid access. TeamId does not match.");
            if (response.ChannelId != payload.Channel.Id) throw new SlackException("Invalid access. ChannelId does not match.");
            if (response.UserId != payload.User.Id) throw new SlackException("Invalid access. UserId does not match.");

            var message = await DatabaseRepo.SelectMessage(response.MessageGuid);
            if (message == null) throw new SlackException("Message not found in database");
            if (!AllowedMessageStates.HasAnyFlags(message.MessageState)) throw new SlackException($"Message is not in a valid state for this action. Message state: {message.MessageState}, valid state: {AllowedMessageStates}");
            if (message.TeamId != payload.Team.Id) throw new SlackException("Invalid access. TeamId does not match.");
            if (message.ChannelId != payload.Channel.Id) throw new SlackException("Invalid access. ChannelId does not match.");
            if (message.UserId != payload.User.Id && message.MessageState == MessageState.Preview) throw new SlackException("Invalid access. UserId does not match.");

            return await Respond(payload, message, response);
        }
    }
}
