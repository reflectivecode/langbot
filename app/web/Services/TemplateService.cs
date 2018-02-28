using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using LangBot.Web.Models;
using LangBot.Web.Slack;
using LangBot.Web.Utilities;
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
                    .WithTypeConverter(new YamlColorConverter())
                    .Build();

                return deserializer.Deserialize<TemplateConfig>(reader);
            }
        }

        public string GetTemplatePath(TemplateConfig.Template template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));

            return Path.Combine(_env.ContentRootPath, _options.Value.ImageDirectory, template.File);
        }

        public string GetFontPath()
        {
            return Path.Combine(_env.ContentRootPath, _options.Value.FontPath);
        }
    }
}
