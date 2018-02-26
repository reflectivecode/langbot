using System;
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
        private readonly Serializer _serializer;
        private readonly ILogger _logger;

        public InteractionService(TokenValidation tokenValidation, IEnumerable<IInteraction> interactions, Serializer serializer, ILogger<InteractionService> logger)
        {
            _tokenValidation = tokenValidation;
            _interactions = interactions;
            _serializer = serializer;
            _logger = logger;
        }

        public async Task<Message> Respond(InteractionRequest request)
        {
            _logger.LogDebug("Interaction payload: {0}", request.Payload);
            var payload = _serializer.JsonToObject<InteractionPayload>(request.Payload);
            _tokenValidation.Validate(payload);
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
            throw new SlackException($"Unhandled CallbackId: {model.CallbackId}, ActionName: {model.ActionName}");
        }
    }
}
