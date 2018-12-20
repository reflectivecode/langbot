using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LangBot.Web.Slack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LangBot.Web.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        private readonly DatabaseRepo _databaseRepo;
        private readonly ILogger<HealthController> _logger;
        private readonly SlackClient _slackClient;

        public HealthController(DatabaseRepo databaseRepo, ILogger<HealthController> logger, SlackClient slackClient)
        {
            _databaseRepo = databaseRepo;
            _logger = logger;
            _slackClient = slackClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var results = await WaitAll(new Dictionary<string, Task<bool>>()
            {
                { "database", Succeeds(_databaseRepo.Test()) },
                { "slack", Succeeds(_slackClient.ApiTest(new SlackApiTestRequest())) },
                { "auth", Succeeds(_slackClient.AuthTest(new SlackApiAuthTestRequest())) },
            });

            var success = results.All(x => x.Value);
            var statusCode = success ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            return base.StatusCode((int)statusCode, results);
        }

        private async Task<bool> Succeeds(Task task)
        {
            try
            {
                await task;
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error running health check");
                return false;
            }
        }

        private async Task<IDictionary<TKey, TValue>> WaitAll<TKey, TValue>(IDictionary<TKey, Task<TValue>> dictionary)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var (key, value) in dictionary)
                result.Add(key, await value);
            return result;
        }
    }
}
