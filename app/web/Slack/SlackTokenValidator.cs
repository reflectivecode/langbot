using System;
using Microsoft.Extensions.Options;

namespace LangBot.Web.Slack
{
    public class SlackTokenValidator
    {
        private readonly IOptions<Options> _options;

        public SlackTokenValidator(IOptions<Options> options)
        {
            _options = options;
        }

        public void Validate(ISlackRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var token = _options.Value.Token;
            if (!string.IsNullOrEmpty(token) && request.Token != token)
                throw new SlackException("Incorrect slack token received.");
        }
    }
}
