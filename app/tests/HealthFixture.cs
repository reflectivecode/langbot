using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace LangBot.Tests
{
    [TestFixture]
    public class HealthFixture : BaseFixture
    {
        [Test]
        public async Task Returns_Ok()
        {
            var response = await Client.GetAsync("/api/health");

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);

            var actual = JObject.Parse(await response.Content.ReadAsStringAsync());
            var expected = JObject.FromObject(new
            {
                database = true,
                slack = false,
                auth = false,
            });
            Assert.AreEqual(expected.ToString(), actual.ToString());
        }
    }
}
