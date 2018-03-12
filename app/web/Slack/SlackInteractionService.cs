using System.Collections.Generic;
using System.Threading.Tasks;
using LangBot.Web.Services;
using Microsoft.Extensions.Logging;

namespace LangBot.Web.Slack
{
    public class SlackInteractionService
    {
        private readonly SlackTokenValidator _tokenValidation;
        private readonly IEnumerable<ISlackInteractionResponder> _interactions;
        private readonly IEnumerable<ISlackDialogResponder> _dialogs;
        private readonly Serializer _serializer;
        private readonly ILogger _logger;

        public SlackInteractionService(SlackTokenValidator tokenValidation, IEnumerable<ISlackInteractionResponder> interactions, IEnumerable<ISlackDialogResponder> dialogs, Serializer serializer, ILogger<SlackInteractionService> logger)
        {
            _tokenValidation = tokenValidation;
            _interactions = interactions;
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

            if (payload is SlackInteractionPayload)
                return await HandleInteraction(payload as SlackInteractionPayload);
            else
                return await HandleDialog(payload as SlackDialogPayload);
        }

        public async Task<SlackDialogResponse> HandleDialog(SlackDialogPayload payload)
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

        public async Task<SlackMessage> HandleInteraction(SlackInteractionPayload payload)
        {
            if (payload == null) throw new System.ArgumentNullException(nameof(payload));

            foreach (var interaction in _interactions)
            {
                var result = await interaction.Respond(payload);
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
