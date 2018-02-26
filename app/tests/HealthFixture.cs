using System;
using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace LangBot.Tests
{
    public class HealthFixture : BaseFixture
    {
        [Fact]
        public async Task Returns_Ok()
        {
            var response = await Client.GetAsync("/api/health");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
