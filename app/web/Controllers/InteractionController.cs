using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LangBot.Web.Slack;

namespace LangBot.Web.Controllers
{
    [Route("api/[controller]")]
    public class InteractionController : Controller
    {
        private readonly SlackInteractionService _service;

        public InteractionController(SlackInteractionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] SlackInteractionRequest request)
        {
            var response = await _service.Respond(request);
            if (response.IsEmptyResponse()) return Ok();
            return Json(response);
        }
    }
}
