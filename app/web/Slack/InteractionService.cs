using System.Collections.Generic;
using System.Threading.Tasks;
using LangBot.Web.Services;
using Microsoft.Extensions.Logging;

namespace LangBot.Web.Slack
{
    public class InteractionService
    {
        private readonly TokenValidation _tokenValidation;
        private readonly IEnumerable<IInteraction> _interactions;
        private readonly IEnumerable<IDialog> _dialogs;
        private readonly Serializer _serializer;
        private readonly ILogger _logger;

        public InteractionService(TokenValidation tokenValidation, IEnumerable<IInteraction> interactions, IEnumerable<IDialog> dialogs, Serializer serializer, ILogger<InteractionService> logger)
        {
            _tokenValidation = tokenValidation;
            _interactions = interactions;
            _dialogs = dialogs;
            _serializer = serializer;
            _logger = logger;
        }

        public async Task<IRequestResponse> Respond(SlackInteractionRequest request)
        {
            _logger.LogDebug("Interaction payload: {0}", request.Payload);
            var payload = _serializer.JsonToObject<IRequestPayload>(request.Payload);
            _tokenValidation.Validate(payload);

            if (payload is SlackInteractionPayload)
                return await HandleInteraction(payload as SlackInteractionPayload);
            else
                return await HandleDialog(payload as SlackDialogPayload);
        }

        public async Task<SlackDialogResponse> HandleDialog(SlackDialogPayload payload)
        {
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
            var model = new InteractionModel(payload);
            foreach (var interaction in _interactions)
            {
                var result = await interaction.Respond(model);
                if (result != null)
                {
                    _logger.LogDebug("Interaction response: {0}", _serializer.ObjectToJson(result));
                    return result;
                }
            }
            throw new SlackException($"Unhandled interaction CallbackId: {model.CallbackId}, ActionName: {model.ActionName}");
        }
    }
}
