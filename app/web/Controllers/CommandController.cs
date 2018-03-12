using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LangBot.Web.Slack;

namespace LangBot.Web.Controllers
{
    [Route("api/[controller]")]
    public class CommandController : Controller
    {
        private readonly SlackCommandService _service;

        public CommandController(SlackCommandService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<SlackMessage> Post([FromForm] SlackCommandRequest request)
        {
            return await _service.Respond(request);
        }
    }
}
