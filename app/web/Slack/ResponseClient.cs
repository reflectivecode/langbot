using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LangBot.Web.Services;
using Microsoft.Extensions.Logging;

namespace LangBot.Web.Slack
{
    public class ResponseClient
    {
        private readonly Serializer _serializer;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public ResponseClient(Serializer serializer, ILogger<InteractionService> logger, HttpClient httpClient)
        {
            _serializer = serializer;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<T> Post<T>(string responseUrl, object payload)
        {
            _logger.LogDebug("Sending to response_url: {0}", responseUrl);
            var json = _serializer.ObjectToJson(payload);
            _logger.LogDebug("body: {0}", json);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (var response = await _httpClient.PostAsync(responseUrl, content))
            {
                if (!response.IsSuccessStatusCode) throw new SlackException($"Failed to post to response_url. Status code {response.StatusCode}");
                var body = await response.Content.ReadAsStringAsync();
                return _serializer.JsonToObject<T>(body);
            }
        }
    }
}
