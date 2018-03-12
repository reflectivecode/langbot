using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LangBot.Web.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LangBot.Web.Slack
{
    public class SlackClient
    {
        private readonly Serializer _serializer;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly IOptions<SlackOptions> _options;

        private string OAuthToken => _options.Value.OAuth.ReturnNullIfEmpty() ?? throw new SlackException("Missing OAuth token configuration");

        public SlackClient(Serializer serializer, ILogger<SlackInteractionService> logger, HttpClient httpClient, IOptions<SlackOptions> options)
        {
            _serializer = serializer;
            _logger = logger;
            _httpClient = httpClient;
            _options = options;
        }

        public async Task<T> Post<T>(string url, object request) where T : ISlackApiResponse
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (request == null) throw new ArgumentNullException(nameof(request));

            _logger.LogDebug("API POST to: {0}", url);
            var json = _serializer.ObjectToJson(request);
            _logger.LogDebug("API request body: {0}", json);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.Add("Authorization", $"Bearer {OAuthToken}");
            using (var response = await _httpClient.PostAsync(url, content))
            {
                if (!response.IsSuccessStatusCode) throw new SlackException($"Failed to post to response_url. Status code {response.StatusCode}");
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("API response body: {0}", body);
                var result = _serializer.JsonToObject<T>(body);
                if (!String.IsNullOrEmpty(result.Warning)) _logger.LogWarning("API warning: {0}", result.Warning);
                if (!String.IsNullOrEmpty(result.Error)) _logger.LogError("API error: {0}", result.Error);
                if (!result.Ok) throw new SlackException("API call failed.");
                return result;
            }
        }

        public async Task<SlackApiTestResponse> ApiTest(SlackApiTestRequest request) => await Post<SlackApiTestResponse>("https://slack.com/api/api.test", request);
        public async Task<SlackApiAuthTestResponse> AuthTest(SlackApiAuthTestRequest request) => await Post<SlackApiAuthTestResponse>("https://api.slack.com/methods/auth.test", request);
        public async Task<SlackApiDialogOpenResponse> DialogOpen(SlackApiDialogOpenRequest request) => await Post<SlackApiDialogOpenResponse>("https://slack.com/api/dialog.open", request);
    }
}
