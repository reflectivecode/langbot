using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LangBot.Web.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LangBot.Web.Slack
{
    public class SlackClient
    {
        private readonly Serializer _serializer;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly IOptions<SlackOptions> _options;
        private readonly JsonSerializer _jsonSerializer;

        private string OAuthToken => _options.Value.OAuth.ReturnNullIfEmpty() ?? throw new SlackException("Missing OAuth token configuration");

        public SlackClient(Serializer serializer, ILogger<SlackInteractionService> logger, HttpClient httpClient, IOptions<SlackOptions> options, JsonSerializer jsonSerializer)
        {
            _serializer = serializer;
            _logger = logger;
            _httpClient = httpClient;
            _options = options;
            _jsonSerializer = jsonSerializer;
        }

        private async Task<T> PostJson<T>(string url, object request) where T : ISlackApiResponse
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (request == null) throw new ArgumentNullException(nameof(request));

            _logger.LogDebug("API POST to: {0}", url);
            var json = _serializer.ObjectToJson(request);
            _logger.LogDebug("API request body: {0}", json);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                return await Post<T>(url, content);
        }

        private async Task<T> PostForm<T>(string url, object request) where T : ISlackApiResponse
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (request == null) throw new ArgumentNullException(nameof(request));

            _logger.LogDebug("API POST to: {0}", url);

            var jObject = JObject.FromObject(request, _jsonSerializer);
            var values = new Dictionary<string, string>();
            foreach (var property in jObject.Properties())
            {
                switch (property.Type)
                {
                    case JTokenType.None:
                    case JTokenType.Undefined:
                        break;
                    case JTokenType.Boolean:
                    case JTokenType.Float:
                    case JTokenType.Integer:
                    case JTokenType.String:
                        values.Add(property.Name, property.Value<string>());
                        break;
                    case JTokenType.Object:
                        values.Add(property.Name, property.ToString());
                        break;
                    default:
                        throw new SlackException($"Unsupported property type. Property: {property.Name} Type: {property.Type}");
                }
            }
            using (var content = new FormUrlEncodedContent(values))
            {
                _logger.LogDebug("API request body: {0}", content.ToString());
                return await Post<T>(url, content);
            }
        }

        private async Task<T> Post<T>(string url, HttpContent content) where T : ISlackApiResponse
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (content == null) throw new ArgumentNullException(nameof(content));

            var requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = content,
                Headers =
                {
                   Authorization = new AuthenticationHeaderValue("Bearer", OAuthToken)
                }
            };

            using (var response = await _httpClient.SendAsync(requestMessage))
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

        public async Task<SlackApiTestResponse> ApiTest(SlackApiTestRequest request) => await PostJson<SlackApiTestResponse>("https://slack.com/api/api.test", request);
        public async Task<SlackApiAuthTestResponse> AuthTest(SlackApiAuthTestRequest request) => await PostJson<SlackApiAuthTestResponse>("https://slack.com/api/auth.test", request);
        public async Task<SlackApiDialogOpenResponse> DialogOpen(SlackApiDialogOpenRequest request) => await PostJson<SlackApiDialogOpenResponse>("https://slack.com/api/dialog.open", request);
        public async Task SendMessageResponse(string responseUrl, SlackMessage message) => await PostJson<SlackApiBaseResponse>(responseUrl, message);
    }
}
