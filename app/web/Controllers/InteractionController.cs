using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LangBot.Web.Slack;

namespace LangBot.Web.Controllers
{
    [Route("api/[controller]")]
    public class InteractionController : Controller
    {
        private readonly InteractionService _service;

        public InteractionController(InteractionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<Message> Post([FromForm] InteractionRequest request)
        {
            return await _service.Respond(request);
        }
    }
}
