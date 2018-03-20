using Xunit;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;

namespace LangBot.Tests
{
    public class HealthFixture : BaseFixture
    {
        [Fact]
        public async Task Returns_Ok()
        {
            var response = await Client.GetAsync("/api/health");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var actual = JObject.Parse(await response.Content.ReadAsStringAsync());
            var expected = JObject.FromObject(new
            {
                database = true,
                slack = false,
                auth = false,
            });
            Assert.Equal(expected.ToString(), actual.ToString());
        }
    }
}
