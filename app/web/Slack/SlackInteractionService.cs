using System.Collections.Generic;
using System.Threading.Tasks;
using LangBot.Web.Services;
using Microsoft.Extensions.Logging;

namespace LangBot.Web.Slack
{
    public class SlackInteractionService
    {
        private readonly SlackTokenValidator _tokenValidation;
        private readonly IEnumerable<ISlackActionResponder> _actions;
        private readonly IEnumerable<ISlackDialogResponder> _dialogs;
        private readonly Serializer _serializer;
        private readonly ILogger _logger;

        public SlackInteractionService(SlackTokenValidator tokenValidation, IEnumerable<ISlackActionResponder> interactions, IEnumerable<ISlackDialogResponder> dialogs, Serializer serializer, ILogger<SlackInteractionService> logger)
        {
            _tokenValidation = tokenValidation;
            _actions = interactions;
            _dialogs = dialogs;
            _serializer = serializer;
            _logger = logger;
        }

        public async Task<ISlackInteractionResponse> Respond(SlackInteractionRequest request)
        {
            if (request == null) throw new System.ArgumentNullException(nameof(request));

            _logger.LogDebug("Interaction payload: {0}", request.Payload);
            var payload = _serializer.JsonToObject<ISlackInteractionPayload>(request.Payload);
            _tokenValidation.Validate(payload);

            if (payload is SlackActionPayload)
                return await HandleAction(payload as SlackActionPayload);
            else
                return await HandleDialog(payload as SlackDialogPayload);

        }

        public async Task<ISlackDialogResponse> HandleDialog(SlackDialogPayload payload)
        {
            if (payload == null) throw new System.ArgumentNullException(nameof(payload));

            foreach (var dialog in _dialogs)
            {
                var result = await dialog.Respond(payload);
                if (result != null)
                {
                    _logger.LogDebug("dialog response: {0}", _serializer.ObjectToJson(result));
                    return result;
                }
            }
            throw new SlackException($"Unhandled dialog CallbackId: {payload.CallbackId}");
        }

        public async Task<ISlackActionResponse> HandleAction(SlackActionPayload payload)
        {
            if (payload == null) throw new System.ArgumentNullException(nameof(payload));

            foreach (var action in _actions)
            {
                var result = await action.Respond(payload);
                if (result != null)
                {
                    _logger.LogDebug("Interaction response: {0}", _serializer.ObjectToJson(result));
                    return result;
                }
            }
            throw new SlackException($"Unhandled interaction CallbackId: {payload.CallbackId}, ActionName: {payload.ActionName}");
        }
    }
}
