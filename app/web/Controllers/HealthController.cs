using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LangBot.Web.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        private readonly DatabaseRepo _databaseRepo;

        public HealthController(DatabaseRepo databaseRepo)
        {
            _databaseRepo = databaseRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            await _databaseRepo.Test();
            return Ok();
        }
    }
}
