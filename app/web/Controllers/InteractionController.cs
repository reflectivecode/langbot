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
        public async Task<IRequestResponse> Post([FromForm] InteractionRequest request)
        {
            var response = await _service.Respond(request);
            if (response.IsEmptyResponse())
                return null;
            return response;
        }
    }
}
