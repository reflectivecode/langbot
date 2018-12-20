using System.Collections.Generic;
using System.Threading.Tasks;
using LangBot.Web.Services;
using Microsoft.Extensions.Logging;

namespace LangBot.Web.Slack
{
    public class SlackCommandService
    {
        private readonly SlackTokenValidator _tokenValidation;
        private readonly IEnumerable<ISlackCommandResponder> _commands;
        private readonly Serializer _serializer;
        private readonly ILogger _logger;

        public SlackCommandService(SlackTokenValidator tokenValidation, IEnumerable<ISlackCommandResponder> commands, Serializer serializer, ILogger<SlackCommandService> logger)
        {
            _tokenValidation = tokenValidation;
            _commands = commands;
            _serializer = serializer;
            _logger = logger;
        }

        public async Task<SlackMessage> Respond(SlackCommandRequest request)
        {
            if (request == null) throw new System.ArgumentNullException(nameof(request));

            _logger.LogDebug("Command payload: {0}", _serializer.ObjectToJson(request));
            _tokenValidation.Validate(request);
            foreach (var command in _commands)
            {
                var result = await command.Respond(request);
                if (result != null)
                {
                    _logger.LogDebug("Command response: {0}", _serializer.ObjectToJson(result));
                    return result;
                }
            }
            throw new SlackException($"Unhandled Command: {request.Command}");
        }
    }
}
