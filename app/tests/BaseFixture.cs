using System;
using System.IO;
using System.Net.Http;
using System.Text;
using LangBot.Web;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LangBot.Tests
{
    public abstract class BaseFixture : IDisposable
    {
        public TestServer Server { get; }
        public HttpClient Client { get; }

        public BaseFixture()
        {
            var path = Path.GetFullPath("../../../../web");
            Server = new TestServer(WebHost.CreateDefaultBuilder().UseContentRoot(path).UseEnvironment("Development").UseStartup<Startup>());
            Client = Server.CreateClient();
        }

        public void Dispose()
        {
            Client.Dispose();
            Server.Dispose();
        }

        public string Base64UrlEncode(object value)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
            var json = JsonConvert.SerializeObject(value, settings);
            var bytes = Encoding.UTF8.GetBytes(json);
            var base64 = Convert.ToBase64String(bytes);
            return base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }
    }
}
