using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace LangBot.Web.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        private readonly IDbConnection _connection;

        public HealthController(IDbConnection connection)
        {
            _connection = connection;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var foo = await _connection.QueryAsync<dynamic>("SELECT * FROM Test;");
            return Ok();
        }
    }
}
