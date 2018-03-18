using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using Boilerplate.AspNetCore;
using Boilerplate.AspNetCore.Filters;
using DbUp;
using DbUp.SQLite.Helpers;
using LangBot.Web.Models;
using LangBot.Web.Slack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LangBot.Web
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            _hostingEnvironment = hostingEnvironment;

            _configuration = new ConfigurationBuilder()
                .SetBasePath(_hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{_hostingEnvironment.EnvironmentName}.json", optional: true)
                // Note: Environment variables use a colon separator e.g. You can override the site title by creating a
                // variable named AppSettings:SiteTitle. See http://docs.asp.net/en/latest/security/app-secrets.html
                .AddEnvironmentVariables("LangBot:")
                .Build();

            loggerFactory.AddConsole(_configuration.GetSection("Logging"));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddResponseCompression()
                .Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal)
                .Configure<LangOptions>(_configuration.GetSection("Lang"))
                .Configure<DatabaseOptions>(_configuration.GetSection("Database"))
                .Configure<Slack.SlackOptions>(_configuration.GetSection("Slack"))
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
        public void Configure(IApplicationBuilder application, IApplicationLifetime lifetime)
        {
            application
                .UseNoServerHttpHeader()
                .UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.All
                })
                .UseHttpException()
                .UseStatusCodePages()
                .UseIfElse(_hostingEnvironment.IsDevelopment(), app => app.UseDeveloperExceptionPage(), app => app.UseInternalServerErrorOnException())
                .UseResponseCompression()
                .UseMvc();

            lifetime.ApplicationStarted.Register(() =>
            {
                var options = application.ApplicationServices.GetRequiredService<IOptions<DatabaseOptions>>();
                if (!String.IsNullOrEmpty(options.Value.DeleteOnStart))
                    File.Delete(options.Value.DeleteOnStart);

                var connection = application.ApplicationServices.GetRequiredService<IDbConnection>();
                var shared = new SharedConnection(connection);
                var upgrader = DeployChanges.To
                            .SQLiteDatabase(shared)
                            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                            .LogToConsole()
                            .Build();

                var result = upgrader.PerformUpgrade();
                if (!result.Successful)
                    throw result.Error;
            });
        }
    }
}
