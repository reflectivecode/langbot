using System;
using Microsoft.AspNetCore.Mvc;

namespace LangBot.Web.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
