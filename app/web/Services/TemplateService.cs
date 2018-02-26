using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using LangBot.Web.Models;
using LangBot.Web.Slack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LangBot.Web.Services
{
    public class TemplateService
    {
        private readonly IOptions<LangOptions> _options;
        private readonly IHostingEnvironment _env;

        public TemplateService(IOptions<LangOptions> options, IHostingEnvironment env)
        {
            _options = options;
            _env = env;
        }

        public async Task<TemplateConfig> GetTemplates()
        {
            var yaml = await File.ReadAllTextAsync(_options.Value.TemplateConfig);
            using (var reader = new StringReader(yaml))
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(new CamelCaseNamingConvention())
                    .Build();

                return deserializer.Deserialize<TemplateConfig>(reader);
            }
        }

        public async Task<TemplateConfig.Template> GetTemplate(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var config = await GetTemplates();
            var template = config.Templates.FirstOrDefault(x => x.Id == id);
            if (template == null) throw new SlackException("Template id not found: {id}");
            return template;
        }

        public async Task<string> GetTemplatePath(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var template = await GetTemplate(id);
            return Path.Combine(_env.ContentRootPath, _options.Value.ImageDirectory, template.File);
        }

        public string GetFontPath()
        {
            return Path.Combine(_env.ContentRootPath, _options.Value.FontPath);
        }
    }
}
