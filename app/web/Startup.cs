using System.Data;
using System.IO.Compression;
using System.Net.Http;
using Boilerplate.AspNetCore;
using Boilerplate.AspNetCore.Filters;
using DbUp.Engine.Output;
using LangBot.Web.Models;
using LangBot.Web.Slack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LangBot.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddResponseCompression()
                .Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal)
                .Configure<LangOptions>(_configuration.GetSection("Lang"))
                .Configure<DatabaseOptions>(_configuration.GetSection("Database"))
                .Configure<SlackOptions>(_configuration.GetSection("Slack"))
                .AddTransient<HttpClient>()
                .AddSingleton(new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    },
                    Converters =
                    {
                        new SlackMessageActionConverter(),
                        new SlackInteractionPayloadConverter(),
                    }
                })
                .AddTransient<IUpgradeLog, UpgradeLog>()
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddTransient(serviceProvider =>
                {
                    var settings = serviceProvider.GetRequiredService<JsonSerializerSettings>();
                    return JsonSerializer.Create(settings);
                })
                .AddTransient<IDbConnection>(serviceProvider =>
                {
                    var options = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>();
                    return new SqliteConnection(options.Value.ConnectionString);
                })
                .AddScoped(serviceProvider =>
                {
                    var urlHelperFactory = serviceProvider.GetRequiredService<IUrlHelperFactory>();
                    var actionContextAccessor = serviceProvider.GetRequiredService<IActionContextAccessor>();
                    return urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
                })
                .AddMvc(config =>
                {
                    config.Filters.Add(new ValidateModelStateAttribute());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(config =>
                {
                    config.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    config.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    };
                });

            var assembly = GetType().Assembly;
            foreach (var type in assembly.DefinedTypes)
            {
                if (type.IsAbstract) continue;
                if (!type.IsClass) continue;

                services.AddTransient(type);

                foreach (var iface in type.GetInterfaces())
                {
                    if (iface.Assembly != assembly) continue;

                    services.AddTransient(iface, type);
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder application)
        {
            application
                .UseNoServerHttpHeader()
                .UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.All
                })
                .UseHttpException()
                .UseStatusCodePages()
                .UseIfElse(_hostingEnvironment.IsDevelopment(),
                    app => app.UseDeveloperExceptionPage(),
                    app => app.UseInternalServerErrorOnException().UseMiddleware<ExceptionLoggingMiddleware>())
                .UseResponseCompression()
                .UseMvc();
        }
    }
}
