using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Boilerplate.AspNetCore;
using Boilerplate.AspNetCore.Filters;
using LangBot.Web.Models;
using LangBot.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.ResponseCompression;
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
                .AddEnvironmentVariables()
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
                .Configure<Slack.Options>(_configuration.GetSection("Slack"))
                .AddSingleton<JsonSerializerSettings>(new JsonSerializerSettings{
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                })
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddTransient<JsonSerializer>(serviceProvider => JsonSerializer.Create(serviceProvider.GetRequiredService<JsonSerializerSettings>()))
                .AddScoped<IUrlHelper>(x => x
                    .GetRequiredService<IUrlHelperFactory>()
                    .GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext))
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
                .UseIfElse(_hostingEnvironment.IsDevelopment(), app => app.UseDeveloperExceptionPage(), app => app.UseInternalServerErrorOnException())
                .UseResponseCompression()
                .UseMvc();
        }
    }
}
